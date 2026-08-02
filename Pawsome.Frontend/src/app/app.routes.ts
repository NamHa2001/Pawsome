import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '',          loadChildren: () => import('./features/san-pham/san-pham.routes').then(m => m.routes) },
  { path: 'tai-khoan', loadChildren: () => import('./features/tai-khoan/tai-khoan.routes').then(m => m.routes) },
  { path: 'gio-hang',  loadChildren: () => import('./features/gio-hang/gio-hang.routes').then(m => m.routes) },
  { path: 'don-hang',  loadChildren: () => import('./features/don-hang/don-hang.routes').then(m => m.routes) },
  { path: 'blog',      loadChildren: () => import('./features/blog-quan-tri/blog.routes').then(m => m.routes) },
  { path: 'wishlist',  loadChildren: () => import('./features/blog-quan-tri/wishlist.routes').then(m => m.routes) },
  { path: 'quan-tri',  loadChildren: () => import('./features/blog-quan-tri/quan-tri.routes').then(m => m.routes) },
];
