import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopicMessageListComponent } from './topic-message-list.component';

describe('TopicMessageListComponent', () => {
    let component: TopicMessageListComponent;
    let fixture: ComponentFixture<TopicMessageListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TopicMessageListComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(TopicMessageListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
