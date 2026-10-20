import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

/** A topic subscription (when `subscription` is set) or a queue. */
export interface MessageTarget {
    entityName: string;
    subscription?: string;
}

@Injectable({ providedIn: 'root' })
export class TopicsService {
    private readonly topicsEndpoint = `${environment.apiBaseUrl}/api/Topics`;
    private readonly queuesEndpoint = `${environment.apiBaseUrl}/api/Queues`;

    constructor(private http: HttpClient) {}

    /**
     * POST /api/Topics/{topicName} ou /api/Queues/{queueName}
     * Envia uma mensagem para o tópico ou fila.
     */
    sendMessage(entityName: string, message: string, isQueue = false): Observable<void> {
        const base = isQueue ? this.queuesEndpoint : this.topicsEndpoint;
        const url = `${base}/${encodeURIComponent(entityName)}`;
        return this.http.post<void>(url, message, {
            headers: { 'Content-Type': 'application/json' }
        });
    }

    /**
     * Lê até max mensagens sem removê-las.
     */
    peekMessages(target: MessageTarget, max: number = 10): Observable<string[]> {
        return this.http.get<string[]>(`${this.targetUrl(target)}/peek`, { params: this.maxParams(max) });
    }

    /**
     * Recebe até max mensagens e as remove da subscription/fila.
     */
    getMessages(target: MessageTarget, max: number = 10): Observable<string[]> {
        return this.http.get<string[]>(this.targetUrl(target), { params: this.maxParams(max) });
    }

    /**
     * Remove todas as mensagens ativas da subscription/fila.
     */
    purge(target: MessageTarget): Observable<{ purged: number }> {
        return this.http.delete<{ purged: number }>(`${this.targetUrl(target)}/messages`);
    }

    private targetUrl(target: MessageTarget): string {
        const entity = encodeURIComponent(target.entityName);
        return target.subscription === undefined
            ? `${this.queuesEndpoint}/${entity}`
            : `${this.topicsEndpoint}/${entity}/${encodeURIComponent(target.subscription)}`;
    }

    private maxParams(max: number): HttpParams {
        return new HttpParams().set('maxMessages', max.toString());
    }
}
