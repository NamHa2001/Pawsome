import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./wishlist/wishlist').then(m => m.Wishlist)
  }
];
