import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TableModule } from 'primeng/table';
import { TextareaModule } from 'primeng/textarea';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { EntitiesService, RuleFilterType, RuleInfo, RuleSettings } from '../../../services/entities.service';
import { LoadingOverlayComponent } from '../../../shared/components/loading-overlay.component';
import { MessageHandlerService } from '../../../shared/services/message-handler.service';

type RulesView = 'list' | 'form';

const correlationFields: { key: keyof RuleSettings; label: string }[] = [
    { key: 'correlationId', label: 'Correlation id' },
    { key: 'messageId', label: 'Message id' },
    { key: 'to', label: 'To' },
    { key: 'replyTo', label: 'Reply to' },
    { key: 'subject', label: 'Subject (label)' },
    { key: 'sessionId', label: 'Session id' },
    { key: 'replyToSessionId', label: 'Reply to session id' },
    { key: 'contentType', label: 'Content type' }
];

/** Lists the rules of a subscription and lets the user create, edit and delete them. */
@Component({
    selector: 'app-rules-dialog',
    standalone: true,
    imports: [FormsModule, ButtonModule, DialogModule, InputTextModule, SelectButtonModule, TableModule, TextareaModule, LoadingOverlayComponent],
    templateUrl: './rules-dialog.component.html'
})
export class RulesDialogComponent {
    @Input() visible = false;
    @Input() topicName = '';
    @Input() subscriptionName = '';
    @Input() rules: RuleInfo[] = [];
    @Output() visibleChange = new EventEmitter<boolean>();
    /** Emitted after a rule was created, updated or deleted, so the parent reloads the entities. */
    @Output() changed = new EventEmitter<void>();

    protected view: RulesView = 'list';
    protected loading = false;
    protected editing = false;
    protected ruleName = '';
    protected values: RuleSettings = {};
    protected applicationPropertiesText = '';
    protected readonly filterTypes: RuleFilterType[] = ['Sql', 'Correlation', 'True', 'False'];
    protected readonly correlationFields = correlationFields;

    private entitiesService = inject(EntitiesService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    protected summary(rule: RuleInfo): string {
        const props = rule.properties;
        if (props.filterType === 'Sql') {
            return props.sqlExpression ?? '';
        }
        if (props.filterType === 'Correlation') {
            const pairs = correlationFields.filter((f) => props[f.key]).map((f) => `${f.key}=${props[f.key]}`);
            const custom = Object.entries(props.applicationProperties ?? {}).map(([k, v]) => `${k}=${v}`);
            return [...pairs, ...custom].join(', ');
        }
        return '';
    }

    protected handleShow(): void {
        this.view = 'list';
    }

    protected newRule(): void {
        this.openForm(false, '', { filterType: 'Sql' });
    }

    protected editRule(rule: RuleInfo): void {
        this.openForm(true, rule.name, rule.properties);
    }

    protected backToList(): void {
        this.view = 'list';
    }

    protected canSave(): boolean {
        const filterType = this.values.filterType;
        if (this.ruleName.trim() === '' || !filterType) {
            return false;
        }
        if (filterType === 'Sql') {
            return (this.values.sqlExpression ?? '').trim() !== '';
        }
        return true;
    }

    protected save(): void {
        const settings = this.buildSettings();
        const name = this.ruleName.trim();
        const request = this.editing
            ? this.entitiesService.updateRule(this.topicName, this.subscriptionName, name, settings)
            : this.entitiesService.createRule(this.topicName, this.subscriptionName, name, settings);

        this.run(request, 'Rule saved successfully!', 'Error saving rule.', () => (this.view = 'list'));
    }

    protected confirmDelete(rule: RuleInfo): void {
        this.confirmation.confirm({
            header: 'Delete rule',
            message: `Delete rule "${rule.name}"? <br><strong>This cannot be undone.</strong>`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Delete', severity: 'danger' },
            rejectButtonProps: { label: 'Cancel', severity: 'secondary', text: true },
            accept: () =>
                this.run(this.entitiesService.deleteRule(this.topicName, this.subscriptionName, rule.name), 'Rule deleted successfully!', 'Error deleting rule.')
        });
    }

    private openForm(editing: boolean, name: string, settings: RuleSettings): void {
        this.editing = editing;
        this.ruleName = name;
        this.values = { ...settings };
        this.applicationPropertiesText = Object.entries(settings.applicationProperties ?? {})
            .map(([key, value]) => `${key}=${value}`)
            .join('\n');
        this.view = 'form';
    }

    private buildSettings(): RuleSettings {
        const settings: RuleSettings = { filterType: this.values.filterType, actionExpression: this.blankToNull(this.values.actionExpression) };

        if (settings.filterType === 'Sql') {
            settings.sqlExpression = this.values.sqlExpression;
        } else if (settings.filterType === 'Correlation') {
            for (const field of correlationFields) {
                (settings as Record<string, unknown>)[field.key] = this.blankToNull(this.values[field.key] as string | null | undefined);
            }
            settings.applicationProperties = this.parseApplicationProperties();
        }

        return settings;
    }

    private parseApplicationProperties(): Record<string, string> {
        const result: Record<string, string> = {};
        for (const line of this.applicationPropertiesText.split('\n')) {
            const separator = line.indexOf('=');
            if (separator > 0) {
                result[line.slice(0, separator).trim()] = line.slice(separator + 1).trim();
            }
        }
        return result;
    }

    private blankToNull(value: string | null | undefined): string | null {
        return value && value.trim() !== '' ? value : null;
    }

    private run(request: Observable<unknown>, successMessage: string, errorTitle: string, onSuccess?: () => void): void {
        this.loading = true;
        request.pipe(finalize(() => (this.loading = false))).subscribe({
            next: () => {
                this.messageHandler.addSuccess(successMessage);
                onSuccess?.();
                this.changed.emit();
            },
            error: (err) => this.messageHandler.handleHttpError(err, errorTitle)
        });
    }
}
