import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputNumber, InputNumberModule } from 'primeng/inputnumber';
import { TabsModule } from 'primeng/tabs';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs/operators';

import { TopicsService } from '../../services/topics.service';
import { EntityNode, TopicListComponent } from './topic-list/topic-list.component';
import { JsonViewerComponent } from './topic-message-list/topic-message-list.component';

@Component({
    selector: 'app-topics',
    imports: [CommonModule, TopicListComponent, InputNumberModule, ButtonModule, FormsModule, JsonViewerComponent, TextareaModule, DividerModule, TabsModule],
    templateUrl: './topics.component.html',
    styleUrl: './topics.component.scss'
})
export class TopicsComponent {
    @ViewChild('messageInput', { static: false })
    private messageInput?: ElementRef<HTMLTextAreaElement>;

    @ViewChild('maxMessagesInput', { static: false })
    private maxMessagesInput!: InputNumber;

    protected selectedEntity?: EntityNode;
    protected messages: string[] = [];
    protected canLoad: boolean = false;
    protected maxMessages: number = 10;
    protected message: string = '';

    private topicsService = inject(TopicsService);

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
                },
                error: (err) => console.log(err)
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
                next: (data) => console.log(data),
                error: (err) => console.log(err)
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
