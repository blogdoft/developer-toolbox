import { Injectable } from '@angular/core';

export interface ProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    // Para ValidationProblemDetails
    errors?: { [field: string]: string[] };
    // Caso venha em extensions
    [key: string]: any;
}

/**
 * Recebe um ProblemDetails (ou ValidationProblemDetails) e retorna
 * um texto amigável, listando status, título, detail e todos os erros.
 */
export function parseProblemDetails(pd: ProblemDetails): string {
    const lines: string[] = [];

    if (pd.title) {
        lines.push(pd.title);
    }
    if (pd.detail) {
        lines.push(pd.detail);
    }

    // ValidationProblemDetails.errors
    if (pd.errors) {
        for (const field of Object.keys(pd.errors)) {
            for (const msg of pd.errors[field]) {
                lines.push(` • ${field}: ${msg}`);
                lines.push('');
            }
        }
    }
    // ou um campo genérico “errors” em extensions
    else if (pd['errors'] && typeof pd['errors'] === 'object') {
        const ext = pd['errors'] as Record<string, any>;
        lines.push('Erros:');
        for (const field of Object.keys(ext)) {
            const arr = ext[field] as string[];
            for (const msg of arr) {
                lines.push(` • ${field}: ${msg}`);
            }
        }
    }

    // Se não encontrou nada acima
    if (lines.length === 0) {
        lines.push('Unknown error happened.');
        if (pd.instance) {
            lines.push(`Instance: ${pd.instance}`);
        } else if (pd.type) {
            lines.push(`Type: ${pd.type}`);
        }
    }

    return lines.join('\n');
}
