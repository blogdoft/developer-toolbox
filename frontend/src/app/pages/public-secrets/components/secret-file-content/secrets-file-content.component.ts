import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';

import { SecretsFile } from '../../../../services/secrets-files.service';
import { AutoSelectAndCopyDirective } from '../../../../shared/directives/auto-select-and-copy-directive';

@Component({
    selector: 'app-secrets-file-content',
    standalone: true,
    imports: [AutoSelectAndCopyDirective, ButtonModule, DialogModule, FormsModule, TextareaModule, ToastModule],
    templateUrl: './secrets-file-content.component.html',
    styleUrl: './secrets-file-content.component.scss'
})
export class SecretsFileContentComponent {
    @Input() visible: boolean = false;
    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() onHide = new EventEmitter<void>();

    @Input() secretsFile: SecretsFile = {
        id: '',
        fileName: '',
        content: ''
    };

    handleHide() {
        this.visibleChange.emit(false);
        this.onHide.emit();
    }
}
