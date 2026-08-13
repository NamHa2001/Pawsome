export interface Address {
  addressId: number;
  nguoiNhan: string;
  soDienThoai: string;
  diaChiChiTiet: string;
  phuongXa: string | null;
  quanHuyen: string | null;
  tinhThanh: string;
  laMacDinh: boolean;
}

export interface CreateAddressRequest {
  nguoiNhan: string;
  soDienThoai: string;
  diaChiChiTiet: string;
  phuongXa?: string;
  quanHuyen?: string;
  tinhThanh: string;
  laMacDinh?: boolean;
}