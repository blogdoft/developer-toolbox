import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MessageService } from 'primeng/api';

import { TopicListComponent } from './topic-list.component';

describe('TopicListComponent', () => {
    let component: TopicListComponent;
    let fixture: ComponentFixture<TopicListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TopicListComponent],
            providers: [provideHttpClient(), provideHttpClientTesting(), MessageService]
        }).compileComponents();

        fixture = TestBed.createComponent(TopicListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
