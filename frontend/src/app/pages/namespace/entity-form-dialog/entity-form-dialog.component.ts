import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';

export type EntityKind = 'topic' | 'subscription' | 'queue';
export type FormMode = 'create' | 'edit';

export interface EntityFormResult {
    name: string;
    settings: Record<string, unknown>;
}

interface FieldDefinition {
    key: string;
    label: string;
    type: 'duration' | 'number' | 'boolean' | 'text';
    /** Can only be set when the entity is created. */
    fixedAfterCreate?: boolean;
}

const durationFields = {
    defaultMessageTimeToLive: { key: 'defaultMessageTimeToLive', label: 'Default message time to live', type: 'duration' },
    autoDeleteOnIdle: { key: 'autoDeleteOnIdle', label: 'Auto delete on idle', type: 'duration' },
    duplicateWindow: { key: 'duplicateDetectionHistoryTimeWindow', label: 'Duplicate detection window', type: 'duration' },
    lockDuration: { key: 'lockDuration', label: 'Lock duration', type: 'duration' }
} satisfies Record<string, FieldDefinition>;

const consumerFields: FieldDefinition[] = [
    durationFields.lockDuration,
    durationFields.defaultMessageTimeToLive,
    durationFields.autoDeleteOnIdle,
    { key: 'maxDeliveryCount', label: 'Max delivery count', type: 'number' },
    { key: 'deadLetteringOnMessageExpiration', label: 'Dead-letter on message expiration', type: 'boolean' },
    { key: 'requiresSession', label: 'Requires session', type: 'boolean', fixedAfterCreate: true },
    { key: 'forwardTo', label: 'Forward to', type: 'text' },
    { key: 'forwardDeadLetteredMessagesTo', label: 'Forward dead-lettered messages to', type: 'text' }
];

const fieldsByKind: Record<EntityKind, FieldDefinition[]> = {
    topic: [
        durationFields.defaultMessageTimeToLive,
        durationFields.duplicateWindow,
        durationFields.autoDeleteOnIdle,
        { key: 'requiresDuplicateDetection', label: 'Requires duplicate detection', type: 'boolean', fixedAfterCreate: true },
        { key: 'maxSizeInMegabytes', label: 'Max size (MB)', type: 'number', fixedAfterCreate: true }
    ],
    subscription: consumerFields,
    queue: [
        ...consumerFields,
        durationFields.duplicateWindow,
        { key: 'requiresDuplicateDetection', label: 'Requires duplicate detection', type: 'boolean', fixedAfterCreate: true },
        { key: 'maxSizeInMegabytes', label: 'Max size (MB)', type: 'number', fixedAfterCreate: true }
    ]
};

/** Create/edit form for topics, subscriptions and queues. */
@Component({
    selector: 'app-entity-form-dialog',
    standalone: true,
    imports: [FormsModule, ButtonModule, CheckboxModule, DialogModule, InputNumberModule, InputTextModule],
    templateUrl: './entity-form-dialog.component.html'
})
export class EntityFormDialogComponent {
    @Input() visible = false;
    @Input() kind: EntityKind = 'topic';
    @Input() mode: FormMode = 'create';
    /** Entity name when editing; ignored when creating. */
    @Input() entityName = '';
    /** Current settings when editing. */
    @Input() initialValues: object = {};
    /** Parent topic name, shown when the entity is a subscription. */
    @Input() parentName?: string;
    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() save = new EventEmitter<EntityFormResult>();

    protected name = '';
    protected values: Record<string, any> = {};

    protected get fields(): FieldDefinition[] {
        return fieldsByKind[this.kind];
    }

    protected get header(): string {
        const action = this.mode === 'create' ? 'New' : 'Edit';
        return `${action} ${this.kind}`;
    }

    protected get canSave(): boolean {
        return this.name.trim() !== '';
    }

    protected isDisabled(field: FieldDefinition): boolean {
        return this.mode === 'edit' && field.fixedAfterCreate === true;
    }

    protected handleShow(): void {
        this.name = this.mode === 'edit' ? this.entityName : '';
        this.values = { ...this.initialValues };
    }

    protected close(): void {
        this.visibleChange.emit(false);
    }

    protected submit(): void {
        const settings: Record<string, unknown> = {};
        for (const field of this.fields) {
            if (this.isDisabled(field)) {
                continue;
            }
            const value = this.values[field.key];
            settings[field.key] = value === '' || value === undefined ? null : value;
        }

        this.save.emit({ name: this.name.trim(), settings });
        this.close();
    }
}
