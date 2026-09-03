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

// pawvip_het_han lưu ngày hết hạn theo NĂM (xem PawVipTiers.ConHieuLuc ở backend) - admin
// vẫn cần thấy các gói đã hết hạn để tra soát, nên so sánh ngay ở đây thay vì lọc từ backend
// như hồ sơ khách tự xem, chỉ để đánh dấu "Expired" cho rõ chứ không ẩn đi. Dùng chung giữa
// trang danh sách (nguoi-dung.ts) và trang chi tiết (chi-tiet-nguoi-dung.ts) để tránh lệch
// logic khi sửa sau này.
export function daHetHan(hetHan: string | null): boolean {
  if (!hetHan) return false;
  return new Date(hetHan) < new Date(new Date().toDateString());
}
