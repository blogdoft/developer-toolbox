import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { TopicsService } from './topics.service';

describe('TopicsService', () => {
    let service: TopicsService;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
        service = TestBed.inject(TopicsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
