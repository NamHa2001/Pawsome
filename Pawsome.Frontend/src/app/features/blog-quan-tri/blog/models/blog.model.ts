export interface BlogPost {
  postId: number;
  tieuDe: string;
  noiDung: string;
  chuDe: string | null;
  tacGiaId: number;
  tenTacGia: string;
  anhDaiDien: string | null;
  ngayDang: string;
}

export interface BlogPostRequest {
  tieuDe: string;
  noiDung: string;
  chuDe: string | null;
  anhDaiDien: string | null;
}

export interface BlogFilterRequest {
  tuKhoa?: string;
  chuDe?: string;
  page?: number;
  pageSize?: number;
}
