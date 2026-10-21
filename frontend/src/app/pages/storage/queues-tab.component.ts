import { DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { StorageQueue, StorageQueueMessage, StorageService } from '../../services/storage.service';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay.component';
import { PromptDialogComponent } from '../../shared/components/prompt-dialog.component';
import { MessageHandlerService } from '../../shared/services/message-handler.service';

type Prompt = 'queue' | 'message' | undefined;

@Component({
    selector: 'app-queues-tab',
    standalone: true,
    imports: [DatePipe, ButtonModule, TableModule, LoadingOverlayComponent, PromptDialogComponent],
    template: `
        <app-loading-overlay [visible]="loading"></app-loading-overlay>
        <app-prompt-dialog
            [header]="prompt === 'queue' ? 'New queue' : 'Send message to ' + selected?.name"
            [label]="prompt === 'queue' ? 'Queue name' : 'Message'"
            [hint]="prompt === 'queue' ? '3 to 63 characters: lowercase letters, numbers and hyphens.' : ''"
            [multiline]="prompt === 'message'"
            [confirmLabel]="prompt === 'queue' ? 'Create' : 'Send'"
            [visible]="prompt !== undefined"
            (visibleChange)="prompt = undefined"
            (confirm)="onPrompt($event)"
        ></app-prompt-dialog>

        <div class="grid grid-cols-12 gap-6">
            <div class="col-span-12 lg:col-span-4">
                <div class="flex items-center justify-between mb-2">
                    <div class="font-semibold">Queues</div>
                    <div class="flex gap-1">
                        <p-button icon="pi pi-refresh" [text]="true" [rounded]="true" ariaLabel="Refresh queues" (onClick)="loadQueues()"></p-button>
                        <p-button icon="pi pi-plus" [text]="true" [rounded]="true" ariaLabel="New queue" (onClick)="prompt = 'queue'"></p-button>
                    </div>
                </div>
                <p-table [value]="queues" dataKey="name" selectionMode="single" [selection]="selected" (selectionChange)="select($event)">
                    <ng-template #body let-queue>
                        <tr [pSelectableRow]="queue">
                            <td>{{ queue.name }}</td>
                            <td>{{ queue.approximateMessageCount }}</td>
                            <td class="text-right">
                                <p-button icon="pi pi-trash" severity="danger" [text]="true" [rounded]="true" ariaLabel="Delete queue" (onClick)="confirmDeleteQueue(queue.name); $event.stopPropagation()"></p-button>
                            </td>
                        </tr>
                    </ng-template>
                    <ng-template #emptymessage><tr><td>No queues.</td></tr></ng-template>
                </p-table>
            </div>

            <div class="col-span-12 lg:col-span-8">
                @if (selected) {
                    <div class="flex items-center justify-between gap-2 mb-2">
                        <div class="font-semibold">{{ selected.name }}</div>
                        <div class="flex gap-2">
                            <p-button icon="pi pi-refresh" severity="secondary" [outlined]="true" label="Peek" (onClick)="loadMessages()"></p-button>
                            <p-button icon="pi pi-send" label="Send" (onClick)="prompt = 'message'"></p-button>
                            <p-button icon="pi pi-trash" severity="danger" [outlined]="true" label="Clear" (onClick)="confirmClear()"></p-button>
                        </div>
                    </div>
                    <p-table [value]="messages" dataKey="messageId">
                        <ng-template #header>
                            <tr><th>Message</th><th>Inserted</th><th>Expires</th><th>Dequeues</th></tr>
                        </ng-template>
                        <ng-template #body let-message>
                            <tr>
                                <td><pre class="m-0 whitespace-pre-wrap break-all">{{ message.body }}</pre></td>
                                <td>{{ message.insertedOn | date: 'short' }}</td>
                                <td>{{ message.expiresOn | date: 'short' }}</td>
                                <td>{{ message.dequeueCount }}</td>
                            </tr>
                        </ng-template>
                        <ng-template #emptymessage><tr><td colspan="4">The queue is empty.</td></tr></ng-template>
                    </p-table>
                    <small>Peek shows up to 32 messages and does not remove them.</small>
                } @else {
                    <div class="text-sm">Select a queue to see its messages.</div>
                }
            </div>
        </div>
    `
})
export class QueuesTabComponent implements OnInit {
    protected queues: StorageQueue[] = [];
    protected messages: StorageQueueMessage[] = [];
    protected selected?: StorageQueue;
    protected prompt: Prompt;
    protected loading = false;

    private storage = inject(StorageService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    ngOnInit(): void {
        this.loadQueues();
    }

    protected loadQueues(): void {
        this.run(this.storage.getQueues(), (queues) => {
            this.queues = queues;
            this.selected = queues.find((q) => q.name === this.selected?.name);
            if (this.selected) {
                this.loadMessages();
            } else {
                this.messages = [];
            }
        }, 'Error loading queues.');
    }

    protected select(queue: StorageQueue | null | undefined): void {
        this.selected = queue ?? undefined;
        this.loadMessages();
    }

    protected loadMessages(): void {
        if (!this.selected) {
            return;
        }
        this.run(this.storage.peekMessages(this.selected.name), (messages) => (this.messages = messages), 'Error reading messages.');
    }

    protected onPrompt(value: string): void {
        const prompt = this.prompt;
        this.prompt = undefined;
        if (prompt === 'queue') {
            this.run(this.storage.createQueue(value), () => {
                this.messageHandler.addSuccess('Queue created successfully!');
                this.loadQueues();
            }, 'Error creating queue.');
        } else if (prompt === 'message' && this.selected) {
            this.run(this.storage.sendMessage(this.selected.name, value), () => {
                this.messageHandler.addSuccess('Message sent successfully!');
                this.loadQueues();
            }, 'Error sending message.');
        }
    }

    protected confirmDeleteQueue(name: string): void {
        this.confirmDelete(`Delete queue "${name}" and its messages?`, () =>
            this.run(this.storage.deleteQueue(name), () => {
                this.messageHandler.addSuccess('Queue deleted successfully!');
                this.loadQueues();
            }, 'Error deleting queue.')
        );
    }

    protected confirmClear(): void {
        const name = this.selected!.name;
        this.confirmDelete(`Remove every message from "${name}"?`, () =>
            this.run(this.storage.clearMessages(name), () => {
                this.messageHandler.addSuccess('Queue cleared successfully!');
                this.loadQueues();
            }, 'Error clearing queue.')
        );
    }

    private confirmDelete(message: string, accept: () => void): void {
        this.confirmation.confirm({
            header: 'Confirm',
            message: `${message} <br><strong>This cannot be undone.</strong>`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Confirm', severity: 'danger' },
            rejectButtonProps: { label: 'Cancel', severity: 'secondary', text: true },
            accept
        });
    }

    private run<T>(request: Observable<T>, next: (value: T) => void, errorTitle: string): void {
        this.loading = true;
        request.pipe(finalize(() => (this.loading = false))).subscribe({
            next,
            error: (err) => this.messageHandler.handleHttpError(err, errorTitle)
        });
    }
}
