import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';

/** Asks the user for a single value, as one line of text or as a multiline text. */
@Component({
    selector: 'app-prompt-dialog',
    standalone: true,
    imports: [FormsModule, ButtonModule, DialogModule, InputTextModule, TextareaModule],
    template: `
        <p-dialog [header]="header" [visible]="visible" (visibleChange)="visibleChange.emit($event)" [modal]="true" [style]="{ width: '36rem' }" (onShow)="value = initialValue">
            <div class="flex flex-col gap-1">
                <label for="promptValue" class="font-semibold">{{ label }}</label>
                @if (multiline) {
                    <textarea pTextarea id="promptValue" rows="12" class="font-mono" [(ngModel)]="value"></textarea>
                } @else {
                    <input pInputText id="promptValue" [(ngModel)]="value" autocomplete="off" (keydown.enter)="submit()" />
                }
                @if (hint) {
                    <small>{{ hint }}</small>
                }
            </div>
            <ng-template #footer>
                <p-button label="Cancel" severity="secondary" [text]="true" (onClick)="visibleChange.emit(false)" />
                <p-button [label]="confirmLabel" [disabled]="!value.trim()" (onClick)="submit()" />
            </ng-template>
        </p-dialog>
    `
})
export class PromptDialogComponent {
    @Input() visible = false;
    @Input() header = '';
    @Input() label = 'Name';
    @Input() hint = '';
    @Input() confirmLabel = 'Save';
    @Input() multiline = false;
    @Input() initialValue = '';
    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() confirm = new EventEmitter<string>();

    protected value = '';

    protected submit(): void {
        if (this.value.trim()) {
            this.confirm.emit(this.value.trim());
        }
    }
}
