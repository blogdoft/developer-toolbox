import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputNumber, InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';
import { TabsModule } from 'primeng/tabs';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { finalize } from 'rxjs/operators';

import { TopicsService } from '../../services/topics.service';
import { MessageHandlerService } from '../../shared/services/message-handler.service';
import { EntityNode, TopicListComponent } from './topic-list/topic-list.component';
import { JsonViewerComponent } from './topic-message-list/topic-message-list.component';

@Component({
    selector: 'app-topics',
    standalone: true,
    imports: [CommonModule, TopicListComponent, InputNumberModule, ButtonModule, FormsModule, JsonViewerComponent, MessageModule, TextareaModule, DividerModule, TabsModule, ToastModule],
    templateUrl: './topics.component.html',
    styleUrl: './topics.component.scss',
    providers: [MessageHandlerService, MessageService]
})
export class TopicsComponent {
    protected selectedEntity?: EntityNode;
    protected messages: string[] = [];
    protected canLoad: boolean = false;
    protected maxMessages: number = 10;
    protected message: string = '';

    private topicsService = inject(TopicsService);
    private messageHandler = inject(MessageHandlerService);

    @ViewChild('messageInput', { static: false })
    private messageInput?: ElementRef<HTMLTextAreaElement>;

    @ViewChild('maxMessagesInput', { static: false })
    private maxMessagesInput!: InputNumber;

    protected onSelectEntity(entityNode: EntityNode): void {
        this.canLoad = false;
        this.selectedEntity = entityNode;
        this.canLoad = this.selectedEntity != null;
    }

    protected load(event: MouseEvent) {
        this.canLoad = false;
        this.topicsService
            .getMessages(this.selectedEntity!.owner!.name, this.selectedEntity!.name, this.maxMessages)
            .pipe(
                finalize(() => {
                    this.canLoad = true;
                })
            )
            .subscribe({
                next: (data: string[]) => {
                    this.messages = data;
                    if (this.message.length === 0) {
                        this.messageHandler.addSuccess('Successfull loading, but the topic is empty');
                        return;
                    }

                    this.messageHandler.addSuccess('Successfull loading!');
                },
                error: (err) => this.messageHandler.handleHttpError(err, 'Error loading messages.')
            });
    }

    protected canSend(): boolean {
        return this.message.trim() != '';
    }

    protected send(event: MouseEvent) {
        this.canLoad = false;
        this.topicsService
            .sendMessage(this.selectedEntity!.owner!.name, this.message)
            .pipe(finalize(() => (this.canLoad = true)))
            .subscribe({
                next: (data) => this.messageHandler.addSuccess('Message sent successfully!'),
                error: (err) => this.messageHandler.handleHttpError(err, 'Error sending message')
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
