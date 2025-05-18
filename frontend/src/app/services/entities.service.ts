import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Topic {
    topicName: string;
    subscriptions: string[];
}

@Injectable({ providedIn: 'root' })
export class EntitiesService {
    private readonly topicsEndpoint = `${environment.apiBaseUrl}/api/Entities`;

    constructor(private http: HttpClient) {}

    /**
     * GET /api/Entities
     * Retorna a lista de tópicos e suas subscriptions.
     */
    getEntities(): Observable<Topic[]> {
        return this.http.get<Topic[]>(this.topicsEndpoint);
    }
}
