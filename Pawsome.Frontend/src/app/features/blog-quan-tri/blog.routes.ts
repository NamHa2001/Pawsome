import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./blog/blog').then(m => m.Blog)
  },
  {
    path: ':id',
    loadComponent: () => import('./blog-chi-tiet/blog-chi-tiet').then(m => m.BlogChiTiet)
  }
];
