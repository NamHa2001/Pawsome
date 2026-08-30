import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./gio-hang/gio-hang').then(m => m.GioHangComponent)
  },
  {
    path: 'dat-hang-tu-dong',
    loadComponent: () => import('./dat-hang-tu-dong/dat-hang-tu-dong').then(m => m.DatHangTuDongComponent)
  },
  {
    path: 'pawvip',
    loadComponent: () => import('./pawvip/pawvip').then(m => m.PawVipComponent)
  },
  {
    path: 'coupon',
    loadComponent: () => import('./coupon/coupon').then(m => m.CouponComponent)
  }
];