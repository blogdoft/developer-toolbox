import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AppMenuitem } from './app.menuitem';

@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, AppMenuitem, RouterModule],
    template: `<ul class="layout-menu">
        <ng-container *ngFor="let item of model; let i = index">
            <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true"></li>
            <li *ngIf="item.separator" class="menu-separator"></li>
        </ng-container>
    </ul> `
})
export class AppMenu {
    model: MenuItem[] = [];

    ngOnInit() {
        this.model = [
            {
                label: 'Home',
                items: [
                    {
                        label: 'Service Bus',
                        icon: 'pi pi-fw pi-envelope',
                        items: [
                            { label: 'Messages', icon: 'pi pi-fw pi-comments', routerLink: ['/'] },
                            { label: 'Namespace', icon: 'pi pi-fw pi-sitemap', routerLink: ['/pages/namespace'] }
                        ]
                    },
                    {
                        label: 'Azure Storage',
                        icon: 'pi pi-fw pi-database',
                        items: [{ label: 'Blobs, Queues and Tables', icon: 'pi pi-fw pi-folder-open', routerLink: ['/pages/storage'] }]
                    },
                    { label: 'Public Secrets', icon: 'pi pi-fw pi-key', routerLink: ['/pages/public-secrets'] }
                ]
            }
        ];
    }
}
