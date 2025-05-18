import { Routes } from '@angular/router';

import { TopicsComponent } from './topics/topics.component';

export default [
    { path: 'topics', component: TopicsComponent },
    { path: '**', redirectTo: '/notfound' }
] as Routes;
