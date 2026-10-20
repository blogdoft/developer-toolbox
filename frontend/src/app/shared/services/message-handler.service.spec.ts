import { TestBed } from '@angular/core/testing';
import { MessageService } from 'primeng/api';

import { MessageHandlerService } from './message-handler.service';

describe('MessageHandlerService', () => {
    let service: MessageHandlerService;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [MessageService] });
        service = TestBed.inject(MessageHandlerService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
