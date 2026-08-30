export interface AdminUser {
  userId: number;
  email: string;
  hoTen: string;
  soDienThoai: string | null;
  diemPawpoints: number;
  pawVipTier: string | null;
  pawVipHetHan: string | null;
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

// Khớp đúng nhãn hiển thị ở trang PawVip cho khách (Basic/Advanced/VIP) - xem
// features/gio-hang/pawvip/models/pawvip-goi.model.ts
export const PAWVIP_TIER_LABELS: Partial<Record<string, string>> = {
  'thuong': 'Basic',
  'nang-cao': 'Advanced',
  'vip': 'VIP'
};
