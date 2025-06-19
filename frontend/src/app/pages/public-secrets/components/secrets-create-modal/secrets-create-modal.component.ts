import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { FluidModule } from 'primeng/fluid';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';

import { NewSecretsFile } from '../../../../services/secrets-files.service';

@Component({
    selector: 'app-secrets-create-modal',
    imports: [ButtonModule, DialogModule, FormsModule, FluidModule, InputTextModule, TextareaModule],
    templateUrl: './secrets-create-modal.component.html',
    styleUrl: './secrets-create-modal.component.scss'
})
export class SecretsCreateModalComponent {
    @Input() visible: boolean = false;
    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() onHide = new EventEmitter<void>();
    @Output() onSave = new EventEmitter<NewSecretsFile>();
    @ViewChild('inputRef') inputRef!: ElementRef<HTMLInputElement>;
    @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;
    protected newSecretsFile: NewSecretsFile = {};

    protected handleShow() {
        this.newSecretsFile = {};
        setTimeout(() => {
            this.inputRef?.nativeElement?.focus();
        });
    }

    protected handleHide() {
        this.visibleChange.emit(false);
        this.onHide.emit();
    }

    protected triggerFileInput(): void {
        this.fileInputRef.nativeElement.click();
    }

    protected handleFileInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = () => {
            this.newSecretsFile.fileName = file.name;
            this.newSecretsFile.content = reader.result?.toString() ?? '';
        };
        reader.readAsText(file);
    }

    protected onCancel() {
        this.newSecretsFile = {};
        this.handleHide();
    }

    protected handleSave() {
        this.onSave.emit(this.newSecretsFile);
        this.handleHide();
    }
}
