import { Routes } from '@angular/router';

import { PublicSecretsComponent } from './public-secrets/public-secrets.component';
import { TopicsComponent } from './topics/topics.component';

export default [
    { path: 'topics', component: TopicsComponent },
    { path: 'public-secrets', component: PublicSecretsComponent, children: [{ path: 'new', component: PublicSecretsComponent }] },
    { path: '**', redirectTo: '/notfound' }
] as Routes;
