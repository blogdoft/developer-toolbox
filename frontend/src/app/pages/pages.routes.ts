import { Routes } from '@angular/router';
import { Documentation } from './documentation/documentation';
import { Crud } from './crud/crud';
import { Empty } from './empty/empty';
import { TopicsComponent } from './topics/topics.component';

export default [
    { path: 'documentation', component: Documentation },
    { path: 'crud', component: Crud },
    { path: 'empty', component: Empty },
    { path: 'topics', component: TopicsComponent },
    { path: '**', redirectTo: '/notfound' }
] as Routes;
