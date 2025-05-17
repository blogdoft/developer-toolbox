import { Component } from '@angular/core';
import { TopicListComponent } from './topic-list/topic-list.component';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FluidModule } from 'primeng/fluid';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { JsonViewerComponent } from './topic-message-list/topic-message-list.component';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-topics',
    imports: [CommonModule, TopicListComponent, InputTextModule, FluidModule, ButtonModule, SelectModule, FormsModule, TextareaModule, JsonViewerComponent],
    templateUrl: './topics.component.html',
    styleUrl: './topics.component.scss'
})
export class TopicsComponent {
    public messages = [
        { message: 'message 1' },
        { message: 'message 2' },
        {
            otherStructure: 'Message Field 1',
            field2: 'Field 2',
            innerObject: {
                innerProperty: 'Value'
            }
        }
    ];
}
