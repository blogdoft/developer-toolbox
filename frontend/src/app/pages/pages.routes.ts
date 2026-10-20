import { Routes } from '@angular/router';

import { NamespaceComponent } from './namespace/namespace.component';
import { PublicSecretsComponent } from './public-secrets/public-secrets.component';
import { StorageComponent } from './storage/storage.component';
import { TopicsComponent } from './topics/topics.component';

export default [
    { path: 'topics', component: TopicsComponent },
    { path: 'namespace', component: NamespaceComponent },
    { path: 'storage', component: StorageComponent },
    { path: 'public-secrets', component: PublicSecretsComponent, children: [{ path: 'new', component: PublicSecretsComponent }] },
    { path: '**', redirectTo: '/notfound' }
] as Routes;
