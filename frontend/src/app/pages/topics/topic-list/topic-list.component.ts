import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService, TreeNode } from 'primeng/api';
import { MessageModule } from 'primeng/message';
import { TreeModule, TreeNodeSelectEvent } from 'primeng/tree';
import { TreeTableModule } from 'primeng/treetable';
import { catchError, forkJoin, of } from 'rxjs';

import { EntitiesService, Queue, RuleInfo, Topic } from '../../../services/entities.service';
import { MessageHandlerService } from '../../../shared/services/message-handler.service';
import { NodeService } from '../../service/node.service';

@Component({
    selector: 'app-topic-list',
    standalone: true,
    imports: [CommonModule, FormsModule, MessageModule, TreeModule, TreeTableModule],
    templateUrl: './topic-list.component.html',
    styleUrl: './topic-list.component.scss',
    providers: [NodeService, MessageHandlerService, MessageService]
})
export class TopicListComponent implements OnInit {
    @Output() OnEntitySelected = new EventEmitter<EntityNode>();

    treeValue: TreeNode[] = [];
    selectedNode?: TreeNode;

    entitiesService = inject(EntitiesService);
    messageHandler = inject(MessageHandlerService);

    ngOnInit(): void {
        forkJoin({
            topics: this.entitiesService.getEntities(),
            queues: this.entitiesService.getQueues().pipe(catchError(() => of([] as Queue[])))
        }).subscribe({
            next: ({ topics, queues }) => {
                this.treeValue = this.buildTree(topics, queues);
            },
            error: (err) => this.messageHandler.handleHttpError(err)
        });
    }

    protected selectEntity(event: TreeNodeSelectEvent) {
        if (!event.node.leaf) {
            this.selectedNode = undefined;
            return;
        }
        this.selectedNode = event.node;
        this.OnEntitySelected.emit(event.node.data);
    }

    private buildTree(topics: Topic[], queues: Queue[]): TreeNode[] {
        return [this.buildTopics(topics), this.buildQueues(queues)];
    }

    private buildQueues(queues: Queue[]): TreeNode {
        return {
            key: '1',
            label: 'Queues',
            icon: 'pi pi-fw pi-inbox',
            selectable: false,
            children: queues.map((q, i) => ({
                key: `1-${i}`,
                label: q.name,
                data: { kind: 'Queue', name: q.name, properties: q.properties },
                icon: 'pi pi-fw pi-list',
                leaf: true
            }))
        };
    }

    private buildTopics(topics: Topic[]): TreeNode {
        const topicNode: TreeNode = {
            key: '0',
            label: 'Topics',
            icon: 'pi pi-fw pi-hashtag',
            selectable: false,
            children: topics.map((t, i) => {
                let topicData: EntityNode = {
                    kind: 'Topic',
                    name: t.topicName,
                    properties: t.properties
                };
                return {
                    key: `0-${i}`,
                    label: t.topicName,
                    icon: 'pi pi-fw pi-tag',
                    selectable: false,
                    children: t.subscriptions.map((s, j) => ({
                        key: `0-${i}-${j}`,
                        label: s.name,
                        data: { kind: 'Subscription', name: s.name, properties: s.properties, rules: s.rules, owner: topicData },
                        icon: 'pi pi-fw pi-bell',
                        leaf: true
                    }))
                };
            })
        };

        return topicNode;
    }
}

export interface EntityNode {
    kind: string;
    name: string;
    properties?: object;
    rules?: RuleInfo[];
    owner?: EntityNode;
}
