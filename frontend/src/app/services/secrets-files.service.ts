import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SecretsFilesService {
    private readonly secretsFileEndpoint = `${environment.apiBaseUrl}/api/Secrets`;

    constructor(private http: HttpClient) {}

    listSecretFiles(): Observable<SecretsFileLookup[]> {
        const url = `${this.secretsFileEndpoint}/`;

        return this.http.get<SecretsFileLookup[]>(url);
    }

    getById(id: string): Observable<SecretsFile> {
        const url = `${this.secretsFileEndpoint}/${encodeURIComponent(id)}`;
        return this.http.get<SecretsFile>(url);
    }

    save(newSecretsFile: NewSecretsFile): Observable<void> {
        const url = `${this.secretsFileEndpoint}/`;
        return this.http.post<void>(url, newSecretsFile);
    }

    delete(id: string): Observable<void> {
        const url = `${this.secretsFileEndpoint}/${encodeURIComponent(id)}`;
        return this.http.delete<void>(url);
    }
}

export interface SecretsFile {
    id: string;
    fileName: string;
    content: string;
}

export interface NewSecretsFile {
    fileName?: string;
    content?: string;
}

export interface SecretsFileLookup {
    id: string;
    fileName: string;
}
