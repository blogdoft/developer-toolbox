import { DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { BlobContainer, BlobItem, StorageService } from '../../services/storage.service';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay.component';
import { PromptDialogComponent } from '../../shared/components/prompt-dialog.component';
import { MessageHandlerService } from '../../shared/services/message-handler.service';

@Component({
    selector: 'app-blobs-tab',
    standalone: true,
    imports: [DatePipe, FormsModule, ButtonModule, InputTextModule, TableModule, LoadingOverlayComponent, PromptDialogComponent],
    template: `
        <app-loading-overlay [visible]="loading"></app-loading-overlay>
        <app-prompt-dialog
            header="New container"
            label="Container name"
            hint="3 to 63 characters: lowercase letters, numbers and hyphens."
            [visible]="creating"
            (visibleChange)="creating = $event"
            (confirm)="createContainer($event)"
        ></app-prompt-dialog>
        <input #picker type="file" class="hidden" (change)="upload(picker)" />

        <div class="grid grid-cols-12 gap-6">
            <div class="col-span-12 lg:col-span-4">
                <div class="flex items-center justify-between mb-2">
                    <div class="font-semibold">Containers</div>
                    <div class="flex gap-1">
                        <p-button icon="pi pi-refresh" [text]="true" [rounded]="true" ariaLabel="Refresh containers" (onClick)="loadContainers()"></p-button>
                        <p-button icon="pi pi-plus" [text]="true" [rounded]="true" ariaLabel="New container" (onClick)="creating = true"></p-button>
                    </div>
                </div>
                <p-table [value]="containers" dataKey="name" selectionMode="single" [selection]="selected" (selectionChange)="select($event)">
                    <ng-template #body let-container>
                        <tr [pSelectableRow]="container">
                            <td>{{ container.name }}</td>
                            <td class="text-right">
                                <p-button icon="pi pi-trash" severity="danger" [text]="true" [rounded]="true" ariaLabel="Delete container" (onClick)="confirmDeleteContainer(container.name); $event.stopPropagation()"></p-button>
                            </td>
                        </tr>
                    </ng-template>
                    <ng-template #emptymessage><tr><td>No containers.</td></tr></ng-template>
                </p-table>
            </div>

            <div class="col-span-12 lg:col-span-8">
                @if (selected) {
                    <div class="flex items-center justify-between gap-2 mb-2">
                        <div class="font-semibold">{{ selected.name }}</div>
                        <div class="flex gap-2">
                            <input pInputText placeholder="Prefix" [(ngModel)]="prefix" (keydown.enter)="loadBlobs()" autocomplete="off" />
                            <p-button icon="pi pi-search" severity="secondary" [outlined]="true" ariaLabel="Search" (onClick)="loadBlobs()"></p-button>
                            <p-button icon="pi pi-upload" label="Upload" (onClick)="picker.click()"></p-button>
                        </div>
                    </div>
                    <p-table [value]="blobs" dataKey="name">
                        <ng-template #header>
                            <tr><th>Name</th><th>Type</th><th>Size</th><th>Modified</th><th></th></tr>
                        </ng-template>
                        <ng-template #body let-blob>
                            <tr>
                                <td>{{ blob.name }}</td>
                                <td>{{ blob.contentType }}</td>
                                <td>{{ blob.size }} B</td>
                                <td>{{ blob.lastModified | date: 'short' }}</td>
                                <td class="text-right">
                                    <a [href]="downloadUrl(blob.name)" download><p-button icon="pi pi-download" [text]="true" [rounded]="true" ariaLabel="Download"></p-button></a>
                                    <p-button icon="pi pi-trash" severity="danger" [text]="true" [rounded]="true" ariaLabel="Delete blob" (onClick)="confirmDeleteBlob(blob)"></p-button>
                                </td>
                            </tr>
                        </ng-template>
                        <ng-template #emptymessage><tr><td colspan="5">No blobs.</td></tr></ng-template>
                    </p-table>
                } @else {
                    <div class="text-sm">Select a container to see its blobs.</div>
                }
            </div>
        </div>
    `
})
export class BlobsTabComponent implements OnInit {
    protected containers: BlobContainer[] = [];
    protected blobs: BlobItem[] = [];
    protected selected?: BlobContainer;
    protected prefix = '';
    protected creating = false;
    protected loading = false;

    private storage = inject(StorageService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    ngOnInit(): void {
        this.loadContainers();
    }

    protected loadContainers(): void {
        this.run(this.storage.getContainers(), (containers) => {
            this.containers = containers;
            this.selected = containers.find((c) => c.name === this.selected?.name);
            if (this.selected) {
                this.loadBlobs();
            } else {
                this.blobs = [];
            }
        }, 'Error loading containers.');
    }

    protected select(container: BlobContainer | null | undefined): void {
        this.selected = container ?? undefined;
        this.prefix = '';
        this.loadBlobs();
    }

    protected loadBlobs(): void {
        if (!this.selected) {
            return;
        }
        this.run(this.storage.getBlobs(this.selected.name, this.prefix.trim()), (blobs) => (this.blobs = blobs), 'Error loading blobs.');
    }

    protected createContainer(name: string): void {
        this.creating = false;
        this.run(this.storage.createContainer(name), () => {
            this.messageHandler.addSuccess('Container created successfully!');
            this.loadContainers();
        }, 'Error creating container.');
    }

    protected confirmDeleteContainer(name: string): void {
        this.confirmDelete(`Delete container "${name}" and every blob in it?`, () =>
            this.run(this.storage.deleteContainer(name), () => {
                this.messageHandler.addSuccess('Container deleted successfully!');
                this.loadContainers();
            }, 'Error deleting container.')
        );
    }

    protected upload(picker: HTMLInputElement): void {
        const file = picker.files?.[0];
        picker.value = '';
        if (!file || !this.selected) {
            return;
        }
        const name = this.prefix.trim() ? `${this.prefix.trim()}${file.name}` : undefined;
        this.run(this.storage.uploadBlob(this.selected.name, file, name), () => {
            this.messageHandler.addSuccess('Blob uploaded successfully!');
            this.loadBlobs();
        }, 'Error uploading blob.');
    }

    protected downloadUrl(name: string): string {
        return this.storage.blobDownloadUrl(this.selected!.name, name);
    }

    protected confirmDeleteBlob(blob: BlobItem): void {
        this.confirmDelete(`Delete blob "${blob.name}"?`, () =>
            this.run(this.storage.deleteBlob(this.selected!.name, blob.name), () => {
                this.messageHandler.addSuccess('Blob deleted successfully!');
                this.loadBlobs();
            }, 'Error deleting blob.')
        );
    }

    private confirmDelete(message: string, accept: () => void): void {
        this.confirmation.confirm({
            header: 'Delete',
            message: `${message} <br><strong>This cannot be undone.</strong>`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Delete', severity: 'danger' },
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
