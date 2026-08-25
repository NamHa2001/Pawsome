import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./quan-tri/admin-layout/admin-layout').then(m => m.AdminLayout),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./quan-tri/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'san-pham',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./quan-tri/san-pham/san-pham').then(m => m.QuanTriSanPham)
      },
      {
        path: 'don-hang',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./quan-tri/don-hang/don-hang').then(m => m.QuanTriDonHang)
      },
      {
        path: 'nguoi-dung',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./quan-tri/nguoi-dung/nguoi-dung').then(m => m.QuanTriNguoiDung)
      },
      {
        path: 'kiem-duyet-danh-gia',
        canActivate: [roleGuard('Admin', 'Moderator')],
        loadComponent: () => import('./quan-tri/kiem-duyet-danh-gia/kiem-duyet-danh-gia').then(m => m.QuanTriKiemDuyet)
      },
      {
        path: 'ma-giam-gia',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('../gio-hang/quan-tri-ma-giam-gia/quan-tri-ma-giam-gia').then(m => m.QuanTriMaGiamGiaComponent)
      },
      {
        path: 'blog',
        canActivate: [roleGuard('Admin')],
        loadComponent: () => import('./quan-tri/blog/quan-tri-blog').then(m => m.QuanTriBlog)
      }
    ]
  }
];
