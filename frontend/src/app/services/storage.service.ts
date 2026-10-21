import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

/** Whether the backend is connected to Azurite or to a real storage account. */
export interface StorageInfo {
    isEmulator: boolean;
    accountName: string;
}

export interface BlobContainer {
    name: string;
    lastModified?: string | null;
    publicAccess?: string | null;
}

export interface BlobItem {
    name: string;
    size?: number | null;
    contentType?: string | null;
    lastModified?: string | null;
}

export interface StorageQueue {
    name: string;
    approximateMessageCount: number;
}

export interface StorageQueueMessage {
    messageId: string;
    body: string;
    insertedOn?: string | null;
    expiresOn?: string | null;
    dequeueCount: number;
}

/** Tables are schemaless, so an entity is a bag of properties. */
export type TableEntity = Record<string, unknown>;

@Injectable({ providedIn: 'root' })
export class StorageService {
    private readonly endpoint = `${environment.apiBaseUrl}/api/storage`;

    constructor(private http: HttpClient) {}

    getInfo(): Observable<StorageInfo> {
        return this.http.get<StorageInfo>(this.endpoint);
    }

    getContainers(): Observable<BlobContainer[]> {
        return this.http.get<BlobContainer[]>(`${this.endpoint}/blobs/containers`);
    }

    createContainer(container: string): Observable<void> {
        return this.http.post<void>(this.containerUrl(container), null);
    }

    deleteContainer(container: string): Observable<void> {
        return this.http.delete<void>(this.containerUrl(container));
    }

    getBlobs(container: string, prefix?: string): Observable<BlobItem[]> {
        let params = new HttpParams();
        if (prefix) {
            params = params.set('prefix', prefix);
        }
        return this.http.get<BlobItem[]>(`${this.containerUrl(container)}/blobs`, { params });
    }

    uploadBlob(container: string, file: File, name?: string): Observable<void> {
        const form = new FormData();
        form.append('file', file);
        if (name) {
            form.append('name', name);
        }
        return this.http.post<void>(`${this.containerUrl(container)}/blobs`, form);
    }

    /** Link that makes the browser download the blob. */
    blobDownloadUrl(container: string, name: string): string {
        return `${this.containerUrl(container)}/blobs/download?name=${encodeURIComponent(name)}`;
    }

    deleteBlob(container: string, name: string): Observable<void> {
        return this.http.delete<void>(`${this.containerUrl(container)}/blobs`, { params: { name } });
    }

    getQueues(): Observable<StorageQueue[]> {
        return this.http.get<StorageQueue[]>(`${this.endpoint}/queues`);
    }

    createQueue(queue: string): Observable<void> {
        return this.http.post<void>(this.queueUrl(queue), null);
    }

    deleteQueue(queue: string): Observable<void> {
        return this.http.delete<void>(this.queueUrl(queue));
    }

    /** Reads messages without removing them. */
    peekMessages(queue: string, maxMessages = 32): Observable<StorageQueueMessage[]> {
        return this.http.get<StorageQueueMessage[]>(`${this.queueUrl(queue)}/messages`, { params: { maxMessages } });
    }

    sendMessage(queue: string, text: string): Observable<void> {
        return this.http.post<void>(`${this.queueUrl(queue)}/messages`, { text });
    }

    clearMessages(queue: string): Observable<void> {
        return this.http.delete<void>(`${this.queueUrl(queue)}/messages`);
    }

    getTables(): Observable<string[]> {
        return this.http.get<string[]>(`${this.endpoint}/tables`);
    }

    createTable(table: string): Observable<void> {
        return this.http.post<void>(this.tableUrl(table), null);
    }

    deleteTable(table: string): Observable<void> {
        return this.http.delete<void>(this.tableUrl(table));
    }

    /** @param filter OData filter, e.g. `PartitionKey eq 'a'`. */
    getEntities(table: string, filter?: string, top = 100): Observable<TableEntity[]> {
        let params = new HttpParams().set('top', top);
        if (filter) {
            params = params.set('filter', filter);
        }
        return this.http.get<TableEntity[]>(`${this.tableUrl(table)}/entities`, { params });
    }

    /** Inserts the entity or replaces the one with the same PartitionKey and RowKey. */
    upsertEntity(table: string, entity: TableEntity): Observable<void> {
        return this.http.put<void>(`${this.tableUrl(table)}/entities`, entity);
    }

    deleteEntity(table: string, partitionKey: string, rowKey: string): Observable<void> {
        return this.http.delete<void>(`${this.tableUrl(table)}/entities`, { params: { partitionKey, rowKey } });
    }

    private containerUrl(container: string): string {
        return `${this.endpoint}/blobs/containers/${encodeURIComponent(container)}`;
    }

    private queueUrl(queue: string): string {
        return `${this.endpoint}/queues/${encodeURIComponent(queue)}`;
    }

    private tableUrl(table: string): string {
        return `${this.endpoint}/tables/${encodeURIComponent(table)}`;
    }
}
