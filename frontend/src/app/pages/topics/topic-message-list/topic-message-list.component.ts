import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'app-json-viewer',
    standalone: true,
    imports: [CommonModule, NgxJsonViewerModule, ButtonModule],
    templateUrl: './topic-message-list.component.html'
})
export class JsonViewerComponent {
    @Input() data: any;
    viewMode: 'tree' | 'text' = 'tree';

    toggleView() {
        this.viewMode = this.viewMode === 'tree' ? 'text' : 'tree';
    }

    copy() {
        const text = JSON.stringify(this.data, null, 2);
        navigator.clipboard.writeText(text);
    }
}
