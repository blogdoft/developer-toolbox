import { Component, inject, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { EntitiesService, Queue, RuleInfo, Subscription, Topic } from '../../services/entities.service';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay.component';
import { MessageHandlerService } from '../../shared/services/message-handler.service';
import { RulesDialogComponent } from './rules-dialog/rules-dialog.component';
import { EntityFormDialogComponent, EntityFormResult, EntityKind, FormMode } from './entity-form-dialog/entity-form-dialog.component';

interface FormState {
    visible: boolean;
    kind: EntityKind;
    mode: FormMode;
    entityName: string;
    parentName?: string;
    initialValues: object;
}

/** Lists the namespace entities and lets the user create, edit and delete them. */
@Component({
    selector: 'app-namespace',
    standalone: true,
    imports: [ButtonModule, ConfirmDialogModule, TableModule, ToastModule, LoadingOverlayComponent, EntityFormDialogComponent, RulesDialogComponent],
    templateUrl: './namespace.component.html',
    providers: [MessageHandlerService, MessageService, ConfirmationService]
})
export class NamespaceComponent implements OnInit {
    protected topics: Topic[] = [];
    protected queues: Queue[] = [];
    protected loading = false;
    protected expandedTopics: Record<string, boolean> = {};
    protected form: FormState = { visible: false, kind: 'topic', mode: 'create', entityName: '', initialValues: {} };

    protected rulesTarget?: { topic: string; subscription: string };

    private entitiesService = inject(EntitiesService);
    private messageHandler = inject(MessageHandlerService);
    private confirmation = inject(ConfirmationService);

    ngOnInit(): void {
        this.refresh();
    }

    protected refresh(): void {
        this.loading = true;
        let pending = 2;
        const done = () => {
            if (--pending === 0) {
                this.loading = false;
            }
        };

        this.entitiesService.getEntities().subscribe({
            next: (data) => (this.topics = data),
            error: (err) => {
                this.messageHandler.handleHttpError(err, 'Error loading topics.');
                done();
            },
            complete: done
        });
        this.entitiesService.getQueues().subscribe({
            next: (data) => (this.queues = data),
            error: (err) => {
                this.messageHandler.handleHttpError(err, 'Error loading queues.');
                done();
            },
            complete: done
        });
    }

    protected openCreate(kind: EntityKind, parent?: Topic): void {
        this.form = { visible: true, kind, mode: 'create', entityName: '', parentName: parent?.topicName, initialValues: {} };
    }

    protected openEdit(kind: EntityKind, name: string, properties: object, parent?: Topic): void {
        this.form = { visible: true, kind, mode: 'edit', entityName: name, parentName: parent?.topicName, initialValues: properties };
    }

    protected get rulesOfTarget(): RuleInfo[] {
        const target = this.rulesTarget;
        return (
            this.topics.find((t) => t.topicName === target?.topic)?.subscriptions.find((s) => s.name === target?.subscription)?.rules ?? []
        );
    }

    protected openRules(topic: Topic, subscription: Subscription): void {
        this.rulesTarget = { topic: topic.topicName, subscription: subscription.name };
    }

    protected onFormVisibleChange(visible: boolean): void {
        this.form = { ...this.form, visible };
    }

    protected onSave(result: EntityFormResult): void {
        const { kind, mode, parentName } = this.form;
        const settings = result.settings;
        const name = result.name;
        let request: Observable<unknown>;

        if (kind === 'topic') {
            request = mode === 'create' ? this.entitiesService.createTopic(name, settings) : this.entitiesService.updateTopic(name, settings);
        } else if (kind === 'subscription') {
            request =
                mode === 'create'
                    ? this.entitiesService.createSubscription(parentName!, name, settings)
                    : this.entitiesService.updateSubscription(parentName!, name, settings);
        } else {
            request = mode === 'create' ? this.entitiesService.createQueue(name, settings) : this.entitiesService.updateQueue(name, settings);
        }

        this.run(request, `${this.capitalize(kind)} saved successfully!`, `Error saving ${kind}.`);
    }

    protected confirmDelete(kind: EntityKind, name: string, parent?: Topic): void {
        this.confirmation.confirm({
            header: `Delete ${kind}`,
            message: `Delete ${kind} "${name}"? <br><strong>This cannot be undone.</strong>`,
            icon: 'pi pi-exclamation-triangle',
            acceptButtonProps: { label: 'Delete', severity: 'danger' },
            rejectButtonProps: { label: 'Cancel', severity: 'secondary', text: true },
            accept: () => {
                const request =
                    kind === 'topic'
                        ? this.entitiesService.deleteTopic(name)
                        : kind === 'subscription'
                            ? this.entitiesService.deleteSubscription(parent!.topicName, name)
                            : this.entitiesService.deleteQueue(name);
                this.run(request, `${this.capitalize(kind)} deleted successfully!`, `Error deleting ${kind}.`);
            }
        });
    }

    private run(request: Observable<unknown>, successMessage: string, errorTitle: string): void {
        this.loading = true;
        request.pipe(finalize(() => (this.loading = false))).subscribe({
            next: () => {
                this.messageHandler.addSuccess(successMessage);
                this.refresh();
            },
            error: (err) => this.messageHandler.handleHttpError(err, errorTitle)
        });
    }

    private capitalize(text: string): string {
        return text.charAt(0).toUpperCase() + text.slice(1);
    }
}
