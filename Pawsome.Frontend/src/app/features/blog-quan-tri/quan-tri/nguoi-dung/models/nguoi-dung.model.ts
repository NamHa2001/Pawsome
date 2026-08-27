export interface AdminUser {
  userId: number;
  email: string;
  hoTen: string;
  soDienThoai: string | null;
  diemPawpoints: number;
  trangThai: string;
  role: string;
  ngayTao: string;
}

export interface AdminUserFilter {
  keyword?: string;
  trangThai?: string;
  page?: number;
  pageSize?: number;
}

export interface RoleOption {
  id: number;
  ten: string;
}

export const ROLE_OPTIONS: RoleOption[] = [
  { id: 1, ten: 'Customer' },
  { id: 2, ten: 'Admin' },
  { id: 3, ten: 'Moderator' },
  { id: 4, ten: 'Support' }
];
