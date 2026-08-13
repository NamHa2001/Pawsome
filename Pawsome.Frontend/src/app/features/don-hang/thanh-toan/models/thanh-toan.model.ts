export interface OrderItem {
  orderItemId: number;
  variantId: number;
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
  orderItems: OrderItem[];
}

export interface CreateOrderRequest {
  addressId: number;
  couponId: number | null;
  donViVanChuyen: string;
}

export interface CreatePaymentResult {
  paymentId: number;
  payUrl: string;
}

export type ShippingCarrier = 'GHN' | 'GHTK' | 'ViettelPost';

export const SHIPPING_CARRIER_LABELS: Record<ShippingCarrier, string> = {
  GHN: 'Giao Hang Nhanh (GHN)',
  GHTK: 'Giao Hang Tiet Kiem (GHTK)',
  ViettelPost: 'Viettel Post'
};

export type PaymentMethod = 'momo' | 'vnpay';