import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { EntitiesService } from './entities.service';

describe('EntitiesService', () => {
    let service: EntitiesService;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
        service = TestBed.inject(EntitiesService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
