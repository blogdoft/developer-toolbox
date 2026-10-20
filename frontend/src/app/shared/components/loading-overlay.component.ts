import { Component, Input } from '@angular/core';

/** Full-screen overlay with a spinner that blocks interaction while `visible` is true. */
@Component({
    selector: 'app-loading-overlay',
    standalone: true,
    template: `
        @if (visible) {
            <div class="loading-overlay" role="alert" aria-busy="true">
                <div class="loading-spinner"></div>
            </div>
        }
    `,
    styles: `
        .loading-overlay {
            position: fixed;
            inset: 0;
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
            background: rgba(0, 0, 0, 0.4);
        }

        .loading-spinner {
            width: 4rem;
            height: 4rem;
            border: 0.4rem solid rgba(255, 255, 255, 0.3);
            border-top-color: #fff;
            border-radius: 50%;
            animation: loading-spin 0.8s linear infinite;
        }

        @keyframes loading-spin {
            to {
                transform: rotate(360deg);
            }
        }
    `
})
export class LoadingOverlayComponent {
    @Input() visible = false;
}
