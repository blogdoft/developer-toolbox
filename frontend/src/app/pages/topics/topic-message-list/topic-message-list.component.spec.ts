import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JsonViewerComponent } from './topic-message-list.component';

describe('JsonViewerComponent', () => {
    let component: JsonViewerComponent;
    let fixture: ComponentFixture<JsonViewerComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [JsonViewerComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(JsonViewerComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should toggle between tree and text views', () => {
        component.toggleView();
        expect(component.viewMode).toBe('text');

        component.toggleView();
        expect(component.viewMode).toBe('tree');
    });
});
