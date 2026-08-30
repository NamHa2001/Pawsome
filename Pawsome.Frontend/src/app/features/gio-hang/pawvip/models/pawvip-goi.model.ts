export type MaGoiPawVip = 'thuong' | 'nang-cao' | 'vip';

export interface GoiPawVip {
  id: MaGoiPawVip;
  ten: string;
  phanTramGiam: number;
  giaNam: number;
  noiBat: boolean;
  quyenLoi: string[];
}

// Dùng chung giữa trang chọn gói (pawvip.ts) và trang thanh toán (pawvip-thanh-toan.ts) -
// khớp đúng PawVipTiers ở backend (Services/GioHang/PawVipTiers.cs).
export const DANH_SACH_GOI_PAWVIP: GoiPawVip[] = [
  {
    id: 'thuong',
    ten: 'Basic Plan',
    phanTramGiam: 0.05,
    giaNam: 594000,
    noiBat: false,
    quyenLoi: ['5% off every order', 'Basic priority support']
  },
  {
    id: 'nang-cao',
    ten: 'Advanced Plan',
    phanTramGiam: 0.1,
    giaNam: 1188000,
    noiBat: false,
    quyenLoi: ['10% off every order', 'Faster priority support', 'Free standard shipping']
  },
  {
    id: 'vip',
    ten: 'VIP Plan',
    phanTramGiam: 0.2,
    giaNam: 2376000,
    noiBat: true,
    quyenLoi: ['20% off every order', 'Top-priority support', 'Free shipping on every order', 'Pet birthday gift']
  }
];
