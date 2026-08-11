import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./trang-chu/trang-chu').then(m => m.TrangChu)
  },
  {
    path: 'san-pham',
    loadComponent: () => import('./danh-sach-san-pham/danh-sach-san-pham').then(m => m.DanhSachSanPham)
  },
  {
    path: 'san-pham/:id',
    loadComponent: () => import('./chi-tiet-san-pham/chi-tiet-san-pham').then(m => m.ChiTietSanPham)
  }
];
