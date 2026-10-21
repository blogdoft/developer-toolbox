import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { StorageService, TableEntity } from '../../services/storage.service';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay.component';
import { PromptDialogComponent } from '../../shared/components/prompt-dialog.component';
import { MessageHandlerService } from '../../shared/services/message-handler.service';

type Prompt = 'table' | 'entity' | undefined;

const newEntityTemplate = '{\n  "PartitionKey": "",\n  "RowKey": ""\n}';

@Component({
    selector: 'app-tables-tab',
    standalone: true,
    imports: [FormsModule, ButtonModule, InputTextModule, TableModule, LoadingOverlayComponent, PromptDialogComponent],
    template: `
        <app-loading-overlay [visible]="loading"></app-loading-overlay>
        <app-prompt-dialog
            [header]="prompt === 'table' ? 'New table' : 'Entity in ' + selected"
            [label]="prompt === 'table' ? 'Table name' : 'Entity (JSON)'"
            [hint]="prompt === 'table' ? '3 to 63 characters: letters and numbers, starting with a letter.' : 'Saving replaces the entity with the same PartitionKey and RowKey.'"
            [multiline]="prompt === 'entity'"
            [initialValue]="entityJson"
            [confirmLabel]="prompt === 'table' ? 'Create' : 'Save'"
            [visible]="prompt !== undefined"
            (visibleChange)="prompt = undefined"
            (confirm)="onPrompt($event)"
        ></app-prompt-dialog>

        <div class="grid grid-cols-12 gap-6">
            <div class="col-span-12 lg:col-span-3">
                <div class="flex items-center justify-between mb-2">
                    <div class="font-semibold">Tables</div>
                    <div class="flex gap-1">
                        <p-button icon="pi pi-refresh" [text]="true" [rounded]="true" ariaLabel="Refresh tables" (onClick)="loadTables()"></p-button>
                        <p-button icon="pi pi-plus" [text]="true" [rounded]="true" ariaLabel="New table" (onClick)="openTablePrompt()"></p-button>
                    </div>
                </div>
                <p-table [value]="tables" selectionMode="single" [selection]="selected" (selectionChange)="select($event)">
                    <ng-template #body let-table>
                        <tr [pSelectableRow]="table">
                            <td>{{ table }}</td>
                            <td class="text-right">
                                <p-button icon="pi pi-trash" severity="danger" [text]="true" [rounded]="true" ariaLabel="Delete table" (onClick)="confirmDeleteTable(table); $event.stopPropagation()"></p-button>
                            </td>
                        </tr>
                    </ng-template>
                    <ng-template #emptymessage><tr><td>No tables.</td></tr></ng-template>
                </p-table>
            </div>

            <div class="col-span-12 lg:col-span-9">
                @if (selected) {
                    <div class="flex items-center justify-between gap-2 mb-2">
                        <div class="font-semibold">{{ selected }}</div>
                        <div class="flex gap-2">
                            <input pInputText class="w-72" placeholder="OData filter, e.g. PartitionKey eq 'a'" [(ngModel)]="filter" (keydown.enter)="loadEntities()" autocomplete="off" />
                            <p-button icon="pi pi-search" severity="secondary" [outlined]="true" ariaLabel="Query" (onClick)="loadEntities()"></p-button>
                            <p-button icon="pi pi-plus" label="Entity" (onClick)="openEntityPrompt()"></p-button>
                        </div>
                    </div>
                    <p-table [value]="entities" [scrollable]="true">
                        <ng-template #header>
                            <tr>
                                @for (column of columns; track column) {
                                    <th>{{ column }}</th>
                                }
                                <th></th>
                            </tr>
                        </ng-template>
                        <ng-template #body let-entity>
                            <tr>
                                @for (column of columns; track column) {
                                    <td>{{ display(entity[column]) }}</td>
                                }
                                <td class="text-right whitespace-nowrap">
                                    <p-button icon="pi pi-pencil" [text]="true" [rounded]="true" ariaLabel="Edit entity" (onClick)="openEntityPrompt(entity)"></p-button>
                                    <p-button icon="pi pi-trash" severity="danger" [text]="true" [rounded]="true" ariaLabel="Delete entity" (onClick)="confirmDeleteEntity(entity)"></p-button>
                                </td>
                            </tr>
                        </ng-template>
                        <ng-template #emptymessage><tr><td [attr.colspan]="columns.length + 1">No entities.</td></tr></ng-template>
                    </p-table>
                    <small>Shows up to 100 entities.</small>
                } @else {
                    <div class="text-sm">Select a table to see its entities.</div>
                }
            </div>
        </div>
    `
})
export class TablesTabComponent implements OnInit {
    protected tables: string[] = [];
    protected entities: TableEntity[] = [];
    protected columns: string[] = [];
    protected selected?: string;
    protected filter = '';
    protected prompt: Prompt;
    protected entityJson = '';
    protected loading = false;

    private storage = inject(StorageService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    ngOnInit(): void {
        this.loadTables();
    }

    protected loadTables(): void {
        this.run(this.storage.getTables(), (tables) => {
            this.tables = tables;
            this.selected = tables.find((t) => t === this.selected);
            if (this.selected) {
                this.loadEntities();
            } else {
                this.setEntities([]);
            }
        }, 'Error loading tables.');
    }

    protected select(table: string | null | undefined): void {
        this.selected = table ?? undefined;
        this.filter = '';
        this.loadEntities();
    }

    protected loadEntities(): void {
        if (!this.selected) {
            return;
        }
        this.run(this.storage.getEntities(this.selected, this.filter.trim()), (entities) => this.setEntities(entities), 'Error loading entities.');
    }

    protected display(value: unknown): string {
        return value !== null && typeof value === 'object' ? JSON.stringify(value) : String(value ?? '');
    }

    protected openTablePrompt(): void {
        this.prompt = 'table';
    }

    protected openEntityPrompt(entity?: TableEntity): void {
        const { Timestamp, ...editable } = entity ?? {};
        this.entityJson = entity ? JSON.stringify(editable, null, 2) : newEntityTemplate;
        this.prompt = 'entity';
    }

    protected onPrompt(value: string): void {
        const prompt = this.prompt;
        this.prompt = undefined;
        if (prompt === 'table') {
            this.run(this.storage.createTable(value), () => {
                this.messageHandler.addSuccess('Table created successfully!');
                this.loadTables();
            }, 'Error creating table.');
        } else if (prompt === 'entity' && this.selected) {
            this.saveEntity(this.selected, value);
        }
    }

    protected confirmDeleteTable(name: string): void {
        this.confirmDelete(`Delete table "${name}" and its entities?`, () =>
            this.run(this.storage.deleteTable(name), () => {
                this.messageHandler.addSuccess('Table deleted successfully!');
                this.loadTables();
            }, 'Error deleting table.')
        );
    }

    protected confirmDeleteEntity(entity: TableEntity): void {
        const table = this.selected!;
        const { PartitionKey, RowKey } = entity as { PartitionKey: string; RowKey: string };
        this.confirmDelete(`Delete entity "${PartitionKey}" / "${RowKey}"?`, () =>
            this.run(this.storage.deleteEntity(table, PartitionKey, RowKey), () => {
                this.messageHandler.addSuccess('Entity deleted successfully!');
                this.loadEntities();
            }, 'Error deleting entity.')
        );
    }

    private saveEntity(table: string, json: string): void {
        let entity: TableEntity;
        try {
            entity = JSON.parse(json);
        } catch {
            this.messageHandler.showError('The entity is not a valid JSON.', 'Error saving entity.');
            return;
        }
        this.run(this.storage.upsertEntity(table, entity), () => {
            this.messageHandler.addSuccess('Entity saved successfully!');
            this.loadEntities();
        }, 'Error saving entity.');
    }

    /** Tables are schemaless, so the columns are the keys found in the entities. */
    private setEntities(entities: TableEntity[]): void {
        const keys = new Set<string>(['PartitionKey', 'RowKey']);
        entities.forEach((entity) => Object.keys(entity).forEach((key) => keys.add(key)));
        keys.delete('Timestamp');
        this.entities = entities;
        this.columns = [...keys, ...(entities.length ? ['Timestamp'] : [])];
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
