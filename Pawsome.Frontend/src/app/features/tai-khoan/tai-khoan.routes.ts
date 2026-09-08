import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'dang-nhap',
    loadComponent: () => import('./dang-nhap/dang-nhap').then(m => m.DangNhapComponent)
  },
  {
    path: 'dang-ky',
    loadComponent: () => import('./dang-ky/dang-ky').then(m => m.DangKyComponent)
  },
  {
    path: 'quen-mat-khau',
    loadComponent: () => import('./quen-mat-khau/quen-mat-khau').then(m => m.QuenMatKhauComponent)
  },
  {
    path: '',
    loadComponent: () => import('./tai-khoan-layout/tai-khoan-layout').then(m => m.TaiKhoanLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard').then(m => m.DashboardComponent),
        canActivate: [authGuard]
      },
      {
        path: 'ho-so',
        loadComponent: () => import('./ho-so/ho-so').then(m => m.HoSoComponent)
      },
      {
        path: 'dia-chi',
        loadComponent: () => import('./dia-chi/dia-chi').then(m => m.DiaChiComponent),
        canActivate: [authGuard]
      }
    ]
  }
];