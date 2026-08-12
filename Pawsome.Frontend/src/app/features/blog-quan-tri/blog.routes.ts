import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./blog/blog').then(m => m.Blog)
  }
];
