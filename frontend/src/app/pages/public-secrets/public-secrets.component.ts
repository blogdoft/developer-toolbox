import { Component, computed, inject, OnInit, signal, ViewChild } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { filter, finalize } from 'rxjs';

import { NewSecretsFile, SecretsFile, SecretsFileLookup, SecretsFilesService } from '../../services/secrets-files.service';
import { MessageHandlerService } from '../../shared/services/message-handler.service';
import { SecretsFileContentComponent } from './components/secret-file-content/secrets-file-content.component';
import { SecretsCreateModalComponent } from './components/secrets-create-modal/secrets-create-modal.component';
import { SecretsListTableComponent } from './components/secrets-list-table/secrets-list-table.component';

@Component({
    selector: 'app-public-secrets',
    standalone: true,
    imports: [ButtonModule, DialogModule, SecretsListTableComponent, SecretsFileContentComponent, ToastModule, ToolbarModule, SecretsCreateModalComponent],
    providers: [MessageHandlerService],
    templateUrl: './public-secrets.component.html',
    styleUrl: './public-secrets.component.scss'
})
export class PublicSecretsComponent implements OnInit {
    @ViewChild(SecretsListTableComponent) tableList!: SecretsListTableComponent;
    protected secretsFileList: SecretsFileLookup[] = [];
    protected showContent: boolean = false;
    protected selectedSecretsFile: SecretsFile = { id: '', fileName: '', content: '' };
    protected addingNewSecretsFile = computed(() => this.currentUrl().endsWith('/new'));

    private route: ActivatedRoute = inject(ActivatedRoute);
    private router: Router = inject(Router);
    private secretFileService = inject(SecretsFilesService);
    private currentUrl = signal(this.router.url);
    private messageHandler = inject(MessageHandlerService);

    ngOnInit() {
        this.loadData();
        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            this.currentUrl.set(this.router.url);
        });
    }

    protected showSecretsFile(secretsFileLookup: SecretsFileLookup): void {
        this.secretFileService.getById(secretsFileLookup.id).subscribe({
            next: (data: SecretsFile) => (this.selectedSecretsFile = data)
        });

        this.showContent = true;
    }

    protected hideSecretsFile(): void {
        this.tableList.clearSelection();
        this.showContent = false;
    }

    protected showNewSecretsFile() {
        this.router.navigate(['new'], { relativeTo: this.route });
    }

    protected onModalHide() {
        const baseUrl = this.router.url.split('/new')[0];
        this.router.navigateByUrl(baseUrl);
    }

    protected deleteFile(secretsFile: SecretsFileLookup) {
        this.secretFileService
            .delete(secretsFile.id)
            .pipe(finalize(() => this.loadData()))
            .subscribe({
                next: () => {
                    this.messageHandler.addSuccess('Deleted successfuly');
                    this.loadData();
                },
                error: (err) => {
                    this.messageHandler.handleHttpError(err);
                    this.loadData();
                }
            });
    }

    protected downloadFile(secretsFile: SecretsFileLookup) {
        this.secretFileService.downloadFile(secretsFile).subscribe({
            next: (data) => {
                const blob = new Blob([data.body!], { type: 'text/plain' });

                const a = document.createElement('a');
                const url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = secretsFile.fileName;
                a.click();
                a.remove();
            },
            error: (err) => this.messageHandler.handleHttpError(err)
        });
    }

    protected onSaveNewSecret(newSecret: NewSecretsFile): void {
        this.secretFileService
            .save(newSecret)
            .pipe(finalize(() => this.loadData()))
            .subscribe({
                next: (_) => this.messageHandler.addSuccess('Secrets save successfully'),
                error: (err) => this.messageHandler.handleHttpError(err, 'Error saving secrets')
            });
    }

    private loadData() {
        this.secretFileService.listSecretFiles().subscribe({
            next: (data: SecretsFileLookup[]) => {
                this.secretsFileList = data;
            },
            error: (err) => this.messageHandler.handleHttpError(err)
        });
    }
}
