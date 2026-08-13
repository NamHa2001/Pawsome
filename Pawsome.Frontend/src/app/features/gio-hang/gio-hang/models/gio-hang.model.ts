// Khớp đúng các DTO bên backend (Pawsome.API/DTOs/GioHang/*.cs)

export interface CartItem {
  cartItemId: number;
  variantId: number;
  tenSanPham: string;
  hinhAnh: string | null;
  thuocTinh: string;
  donGia: number;
  donGiaGoc?: number | null;
  soLuong: number;
  thanhTien: number;
}

export interface Cart {
  cartId: number;
  items: CartItem[];
  maCouponDangApDung: string | null;
  giamGia: number;
  phiVanChuyenTamTinh: number;
  tienHang: number;
  tongTien: number;
}

export interface AddCartItem {
  variantId: number;
  soLuong: number;
}

export interface UpdateCartItem {
  soLuong: number;
}

export interface ApplyCoupon {
  maCode: string;
}

export interface ApplyCouponResult {
  hopLe: boolean;
  thongBao: string | null;
  tienHang: number;
  giamGia: number;
  phiVanChuyenTamTinh: number;
  tongTien: number;
}

export interface Coupon {
  couponId: number;
  maCode: string;
  loaiGiam: string;
  giaTri: number;
  ngayBatDau: string | null;
  ngayKetThuc: string | null;
  soLuong: number | null;
  dangHieuLuc: boolean;
}

export type LoaiGiamCoupon = 'percent' | 'fixed';

export interface CreateCoupon {
  maCode: string;
  loaiGiam: LoaiGiamCoupon;
  giaTri: number;
  ngayBatDau: string | null;
  ngayKetThuc: string | null;
  soLuong: number | null;
}

export interface UpdateCoupon {
  loaiGiam: LoaiGiamCoupon;
  giaTri: number;
  ngayBatDau: string | null;
  ngayKetThuc: string | null;
  soLuong: number | null;
}

export type TanSuatDonTuDong = 'weekly' | 'monthly' | 'quarterly' | 'yearly';
export type TrangThaiAutoOrder = 'active' | 'paused' | 'cancelled';

export interface AutoOrder {
  autoOrderId: number;
  variantId: number;
  tenSanPham: string;
  hinhAnh: string | null;
  soLuong: number;
  tanSuat: TanSuatDonTuDong;
  ngayKeTiep: string;
  trangThai: TrangThaiAutoOrder;
}

export interface CreateAutoOrder {
  variantId: number;
  soLuong: number;
  tanSuat: TanSuatDonTuDong;
}

export interface UpdateAutoOrder {
  soLuong: number;
  tanSuat: TanSuatDonTuDong;
}

export const NHAN_TAN_SUAT: Record<TanSuatDonTuDong, string> = {
  weekly: 'Weekly',
  monthly: 'Monthly',
  quarterly: 'Quarterly',
  yearly: 'Yearly'
};

export const NHAN_TRANG_THAI_AUTO_ORDER: Record<TrangThaiAutoOrder, string> = {
  active: 'Active',
  paused: 'Paused',
  cancelled: 'Cancelled'
};