export interface WishlistItem {
  wishlistId: number;
  productId: number;
  tenSanPham: string;
  anhChinh: string | null;
  giaTu: number | null;
  dangKinhDoanh: boolean;
  ngayThem: string;
}
