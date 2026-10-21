import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MessageService } from 'primeng/api';

import { TopicsComponent } from './topics.component';

describe('TopicsComponent', () => {
    let component: TopicsComponent;
    let fixture: ComponentFixture<TopicsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TopicsComponent],
            providers: [provideHttpClient(), provideHttpClientTesting(), MessageService]
        }).compileComponents();

        fixture = TestBed.createComponent(TopicsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
