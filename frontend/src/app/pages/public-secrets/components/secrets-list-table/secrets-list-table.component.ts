import { Component, EventEmitter, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule, TableRowSelectEvent } from 'primeng/table';

import { SecretsFileLookup } from '../../../../services/secrets-files.service';

@Component({
    selector: 'app-secrets-list-table',
    standalone: true,
    imports: [TableModule, FormsModule, ButtonModule],
    templateUrl: './secrets-list-table.component.html',
    styleUrl: './secrets-list-table.component.scss'
})
export class SecretsListTableComponent {
    @Input() secretList: SecretsFileLookup[] = [];

    @Output() onFileSelected = new EventEmitter<SecretsFileLookup>();

    @Output() onDeleteClick = new EventEmitter<SecretsFileLookup>();

    @Output() onDownloadClick = new EventEmitter<SecretsFileLookup>();

    protected selectedRow?: SecretsFileLookup | SecretsFileLookup[];

    public clearSelection() {
        this.selectedRow = [];
    }

    protected rowSelect($event: TableRowSelectEvent<SecretsFileLookup>) {
        const selectedSecretsFile = Array.isArray($event.data) ? $event.data[0] : $event.data;
        this.onFileSelected.emit(selectedSecretsFile);
    }

    protected deleteClickHandle(secretsFile: SecretsFileLookup) {
        this.onDeleteClick.emit(secretsFile);
    }

    protected downloadClickHandle(secretsFile: SecretsFileLookup) {
        this.onDownloadClick.emit(secretsFile);
    }
}
