import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TopicsService {
    private readonly topicsEndpoint = `${environment.apiBaseUrl}/api/Topics`;

    constructor(private http: HttpClient) {}

    /**
     * POST /api/Topics/{topicName}
     * Envia uma mensagem para o tópico.
     */
    sendMessage(topicName: string, message: string): Observable<void> {
        const url = `${this.topicsEndpoint}/${encodeURIComponent(topicName)}`;
        return this.http.post<void>(url, message, {
            headers: { 'Content-Type': 'application/json' }
        });
    }

    /**
     * GET /api/Topics/{topicName}/{subscription}?maxMessages={max}
     * Recebe até max mensagens da subscription.
     */
    getMessages(topicName: string, subscription: string, max: number = 10): Observable<string[]> {
        const url = `${this.topicsEndpoint}/${encodeURIComponent(topicName)}/${encodeURIComponent(subscription)}`;
        const params = new HttpParams().set('maxMessages', max.toString());
        return this.http.get<string[]>(url, { params });
    }
}
