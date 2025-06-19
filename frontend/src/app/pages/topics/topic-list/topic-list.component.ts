import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService, TreeNode } from 'primeng/api';
import { MessageModule } from 'primeng/message';
import { TreeModule, TreeNodeSelectEvent } from 'primeng/tree';
import { TreeTableModule } from 'primeng/treetable';

import { EntitiesService, Topic } from '../../../services/entities.service';
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
        this.entitiesService.getEntities().subscribe({
            next: (data) => {
                this.treeValue = this.buildTree(data);
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

    private buildTree(topics: Topic[]): TreeNode[] {
        const topicNode = this.buildTopics(topics);
        return [topicNode];
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
                    name: t.topicName
                };
                return {
                    key: `0-${i}`,
                    label: t.topicName,
                    icon: 'pi pi-fw pi-tag',
                    selectable: false,
                    children: t.subscriptions.map((s, j) => ({
                        key: `0-${i}-${j}`,
                        label: s,
                        data: { kind: 'Subscription', name: s, owner: topicData },
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
    owner?: EntityNode;
}
