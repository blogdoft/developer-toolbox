import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../environments/environment';
import { StorageService } from './storage.service';

describe('StorageService', () => {
    let service: StorageService;
    let http: HttpTestingController;
    const endpoint = `${environment.apiBaseUrl}/api/storage`;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
        service = TestBed.inject(StorageService);
        http = TestBed.inject(HttpTestingController);
    });

    afterEach(() => http.verify());

    it('should encode the blob name of the download link', () => {
        expect(service.blobDownloadUrl('my container', 'dir/a b.txt')).toBe(
            `${endpoint}/blobs/containers/my%20container/blobs/download?name=dir%2Fa%20b.txt`
        );
    });

    it('should filter blobs by prefix', () => {
        service.getBlobs('docs', 'dir/').subscribe();

        const req = http.expectOne((r) => r.url === `${endpoint}/blobs/containers/docs/blobs`);
        expect(req.request.params.get('prefix')).toBe('dir/');
        req.flush([]);
    });

    it('should send the queue message as text', () => {
        service.sendMessage('orders', '{"a":1}').subscribe();

        const req = http.expectOne(`${endpoint}/queues/orders/messages`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual({ text: '{"a":1}' });
        req.flush(null);
    });

    it('should query entities with filter and top', () => {
        service.getEntities('people', "PartitionKey eq 'a'", 50).subscribe();

        const req = http.expectOne((r) => r.url === `${endpoint}/tables/people/entities`);
        expect(req.request.params.get('filter')).toBe("PartitionKey eq 'a'");
        expect(req.request.params.get('top')).toBe('50');
        req.flush([]);
    });
});
