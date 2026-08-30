export interface AdminOrderItem {
  orderItemId: number;
  variantId: number;
  tenSanPham: string;
  tenBienThe: string;
  soLuong: number;
  donGia: number;
}

export interface AdminOrder {
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
  hoTenKhachHang: string | null;
  emailKhachHang: string | null;
  orderItems: AdminOrderItem[];
}

export interface AdminOrderFilter {
  trangThai?: string;
  userId?: number;
  page?: number;
  pageSize?: number;
}
