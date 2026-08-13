import { Routes } from '@angular/router';

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
  }
  // 'quen-mat-khau', 'dia-chi' sẽ thêm ở bước sau
];