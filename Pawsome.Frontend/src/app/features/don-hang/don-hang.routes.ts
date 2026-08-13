import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'thanh-toan',
    loadComponent: () => import('./thanh-toan/thanh-toan').then(m => m.ThanhToanComponent),
    canActivate: [authGuard]
  },
  {
    path: 'lich-su',
    loadComponent: () => import('./lich-su-don-hang/lich-su-don-hang').then(m => m.LichSuDonHangComponent),
    canActivate: [authGuard]
  }
];