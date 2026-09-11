export interface AuditLog {
  logId: number;
  userId: number | null;
  hoTenNguoiDung: string | null;
  hanhDong: string;
  doiTuong: string | null;
  doiTuongId: number | null;
  chiTiet: string | null;
  diaChiIp: string | null;
  ngayTao: string;
}

export interface AuditLogFilter {
  userId?: number;
  hanhDong?: string;
  tuNgay?: string;
  denNgay?: string;
  page?: number;
  pageSize?: number;
}
