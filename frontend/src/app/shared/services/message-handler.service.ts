import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { parseProblemDetails, ProblemDetails } from './problem-details-handler.service';

@Injectable({
    providedIn: 'root'
})
export class MessageHandlerService {
    constructor(private message: MessageService) {}

    public addSuccess(text: string, title?: string | null) {
        this.message.add({
            summary: title ?? 'Success!',
            detail: text,
            severity: 'success',
            life: 3000
        });
    }

    public handleHttpError(err: any, title?: string | null) {
        let errorMessage = this.getMessageByStatus(err);
        this.showError(errorMessage, title ?? 'Error during request.');
        console.log(JSON.stringify(err));
    }

    public showError(errorMessage: string, title?: string | null) {
        this.message.add({
            detail: errorMessage,
            summary: title ?? 'Error Unknown.',
            severity: 'error',
            life: 5000
        });
    }

    private getMessageByStatus(err: any): string {
        switch (err.status) {
            case 400:
                return parseProblemDetails(err.error as ProblemDetails);
            case 401:
                return 'You must logged in to access this resource.';
            case 403:
                return 'You has no rights to access the request ressource.';
            case 404:
                return 'The requested data does not exists.';
            case 502:
                return 'The server is off-line. Try again later.';
            case 500:
                return `Internal server error \n${JSON.stringify(err)}`;
            default:
                return err.message;
        }
    }
}
