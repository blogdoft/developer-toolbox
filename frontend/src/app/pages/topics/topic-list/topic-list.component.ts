import { Component, inject, OnInit } from '@angular/core';
import { NodeService } from '../../service/node.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeModule } from 'primeng/tree';
import { TreeTableModule } from 'primeng/treetable';
import { TreeNode } from 'primeng/api';

@Component({
    selector: 'app-topic-list',
    imports: [CommonModule, FormsModule, TreeModule, TreeTableModule],
    templateUrl: './topic-list.component.html',
    styleUrl: './topic-list.component.scss',
    providers: [NodeService]
})
export class TopicListComponent implements OnInit {
    treeValue: TreeNode[] = [];

    treeTableValue: TreeNode[] = [];

    selectedTreeValue: TreeNode[] = [];

    selectedTreeTableValue = {};

    cols: any[] = [];

    nodeService = inject(NodeService);

    ngOnInit(): void {
        this.nodeService.getFiles().then((files) => (this.treeValue = files));

        this.selectedTreeTableValue = {
            '0-0': {
                partialChecked: false,
                checked: true
            }
        };
    }
}
