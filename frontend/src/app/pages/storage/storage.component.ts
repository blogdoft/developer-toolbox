import { Component, inject, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TabsModule } from 'primeng/tabs';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';

import { StorageInfo, StorageService } from '../../services/storage.service';
import { MessageHandlerService } from '../../shared/services/message-handler.service';
import { BlobsTabComponent } from './blobs-tab.component';
import { QueuesTabComponent } from './queues-tab.component';
import { TablesTabComponent } from './tables-tab.component';

/** Browses and manages the blobs, queues and tables of Azurite or of a real storage account. */
@Component({
    selector: 'app-storage',
    standalone: true,
    imports: [ConfirmDialogModule, TabsModule, TagModule, ToastModule, BlobsTabComponent, QueuesTabComponent, TablesTabComponent],
    template: `
        <p-toast></p-toast>
        <p-confirmdialog></p-confirmdialog>
        <div class="card">
            <div class="flex items-center gap-3 mb-4">
                <div class="font-semibold text-xl">Storage</div>
                @if (info) {
                    <p-tag [value]="info.isEmulator ? 'Azurite' : 'Azure'" [severity]="info.isEmulator ? 'info' : 'warn'"></p-tag>
                    <span class="text-sm">{{ info.accountName }}</span>
                }
            </div>
            <p-tabs value="blobs">
                <p-tablist>
                    <p-tab value="blobs">Blobs</p-tab>
                    <p-tab value="queues">Queues</p-tab>
                    <p-tab value="tables">Tables</p-tab>
                </p-tablist>
                <p-tabpanels>
                    <p-tabpanel value="blobs"><app-blobs-tab></app-blobs-tab></p-tabpanel>
                    <p-tabpanel value="queues"><app-queues-tab></app-queues-tab></p-tabpanel>
                    <p-tabpanel value="tables"><app-tables-tab></app-tables-tab></p-tabpanel>
                </p-tabpanels>
            </p-tabs>
        </div>
    `,
    providers: [MessageHandlerService, MessageService, ConfirmationService]
})
export class StorageComponent implements OnInit {
    protected info?: StorageInfo;

    private storage = inject(StorageService);
    private messageHandler = inject(MessageHandlerService);

    ngOnInit(): void {
        this.storage.getInfo().subscribe({
            next: (info) => (this.info = info),
            error: (err) => this.messageHandler.handleHttpError(err, 'Error loading storage account.')
        });
    }
}
