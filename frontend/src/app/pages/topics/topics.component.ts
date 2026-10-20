import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AccordionModule } from 'primeng/accordion';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DividerModule } from 'primeng/divider';
import { InputNumber, InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';
import { TabsModule } from 'primeng/tabs';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { MessageTarget, TopicsService } from '../../services/topics.service';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay.component';
import { MessageHandlerService } from '../../shared/services/message-handler.service';
import { EntityNode, TopicListComponent } from './topic-list/topic-list.component';
import { JsonViewerComponent } from './topic-message-list/topic-message-list.component';

@Component({
    selector: 'app-topics',
    standalone: true,
    imports: [CommonModule, AccordionModule, TopicListComponent, InputNumberModule, ButtonModule, FormsModule, JsonViewerComponent, MessageModule, TextareaModule, DividerModule, TabsModule, ToastModule, ConfirmDialogModule, TooltipModule, LoadingOverlayComponent],
    templateUrl: './topics.component.html',
    styleUrl: './topics.component.scss',
    providers: [MessageHandlerService, MessageService, ConfirmationService]
})
export class TopicsComponent {
    protected selectedEntity?: EntityNode;
    protected messages: string[] = [];
    protected canLoad: boolean = false;
    protected maxMessages: number = 10;
    protected message: string = '';
    protected loading: boolean = false;

    private topicsService = inject(TopicsService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    @ViewChild('messageInput', { static: false })
    private messageInput?: ElementRef<HTMLTextAreaElement>;

    @ViewChild('maxMessagesInput', { static: false })
    private maxMessagesInput!: InputNumber;

    protected onSelectEntity(entityNode: EntityNode): void {
        this.canLoad = false;
        this.selectedEntity = entityNode;
        this.canLoad = this.selectedEntity != null;
    }

    protected isQueue(): boolean {
        return this.selectedEntity?.kind === 'Queue';
    }

    protected peek(): void {
        this.fetchMessages(this.topicsService.peekMessages(this.target(), this.maxMessages), 'Peek');
    }

    protected confirmLoad(): void {
        this.confirmation.confirm({
            header: 'Load messages',
            message: `Load removes up to ${this.maxMessages} messages from "${this.selectedEntity!.name}".<br>Use Peek to read without removing.<br>Continue?`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Load', severity: 'danger' },
            rejectButtonProps: { label: 'Cancel', severity: 'secondary', text: true },
            accept: () => this.fetchMessages(this.topicsService.getMessages(this.target(), this.maxMessages), 'Load')
        });
    }

    protected confirmPurge(): void {
        this.confirmation.confirm({
            header: 'Purge',
            message: `Remove ALL active messages from "${this.selectedEntity!.name}"? <br><strong>This cannot be undone.</strong>`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Purge', severity: 'danger' },
            rejectButtonProps: { label: 'Cancel', severity: 'secondary', text: true },
            accept: () =>
                this.run(this.topicsService.purge(this.target()), 'Error purging messages.', (result) => {
                    this.messages = [];
                    this.messageHandler.addSuccess(`${result.purged} message(s) purged.`);
                })
        });
    }

    protected canSend(): boolean {
        return this.message.trim() != '';
    }

    protected send(event: MouseEvent) {
        const entityName = this.isQueue() ? this.selectedEntity!.name : this.selectedEntity!.owner!.name;
        this.run(this.topicsService.sendMessage(entityName, this.message, this.isQueue()), 'Error sending message', () =>
            this.messageHandler.addSuccess('Message sent successfully!')
        );
    }

    private target(): MessageTarget {
        const entity = this.selectedEntity!;
        return this.isQueue() ? { entityName: entity.name } : { entityName: entity.owner!.name, subscription: entity.name };
    }

    private fetchMessages(request: Observable<string[]>, action: string): void {
        this.run(request, `Error: ${action.toLowerCase()} failed.`, (data) => {
            this.messages = data;
            this.messageHandler.addSuccess(data.length === 0 ? `${action} done, but there are no messages.` : `${action} done: ${data.length} message(s).`);
        });
    }

    private run<T>(request: Observable<T>, errorTitle: string, onSuccess: (result: T) => void): void {
        this.canLoad = false;
        this.loading = true;
        request
            .pipe(
                finalize(() => {
                    this.canLoad = true;
                    this.loading = false;
                })
            )
            .subscribe({
                next: onSuccess,
                error: (err) => this.messageHandler.handleHttpError(err, errorTitle)
            });
    }

    protected onTabChange(event: string | number) {
        switch (event) {
            case '0': // load messages
                setTimeout(() => {
                    this.maxMessagesInput.input.nativeElement.focus();
                    this.maxMessagesInput.input.nativeElement.select();
                }, 0);
                break;
            case '1': // Send message
                setTimeout(() => {
                    this.messageInput?.nativeElement.focus();
                }, 0);
                break;
            default:
                break;
        }
    }
}
