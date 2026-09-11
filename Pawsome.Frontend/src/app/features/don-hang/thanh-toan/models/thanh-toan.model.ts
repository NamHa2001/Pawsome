export interface OrderItem {
  orderItemId: number;
  variantId: number;
  productId: number;
  tenSanPham: string;
  tenBienThe: string;
  soLuong: number;
  donGia: number;
  thanhTien: number;
}

export interface Order {
  orderId: number;
  userId: number;
  addressId: number;
  couponId: number | null;
  ngayDat: string;
  tienHang: number;
  phiVanChuyen: number;
  giamGia: number;
  thanhTien: number;
  trangThai: string;
  donViVanChuyen: string | null;
  maVanDon: string | null;
  coTheThanhToan: boolean;
  orderItems: OrderItem[];
}

export interface CreateOrderRequest {
  addressId: number;
  couponId: number | null;
  donViVanChuyen: string;
  soDiemMuonDoi?: number | null;
}

export interface CreatePaymentResult {
  paymentId: number;
  payUrl: string;
}

export type ShippingCarrier = 'Free' | 'GHTK' | 'GHN';

export const SHIPPING_CARRIER_LABELS: Record<ShippingCarrier, string> = {
  Free: 'Free Shipping',
  GHTK: 'Giao Hang Tiet Kiem (GHTK)',
  GHN: 'Giao Hang Nhanh (GHN)'
};

export type PaymentMethod = 'momo' | 'vnpay';

// Dùng cho trang Order History (lịch sử đơn hàng)
export interface OrderFilter {
  trangThai?: string;
  page?: number;
  pageSize?: number;
}

export interface CancelOrderRequest {
  lyDo?: string;
}

export interface ReturnOrderRequest {
  lyDo: string;
}