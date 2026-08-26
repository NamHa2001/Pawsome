import { Component, input } from '@angular/core';

// Tách "6 Reasons to Choose..." thành component riêng (giống banner-noi-bat/deal-thang) - nội
// dung tĩnh giống hệt file mẫu, chỉ cần truyền tên sản phẩm vào tiêu đề. Tách ra để giảm CSS của
// chi-tiet-san-pham.scss xuống dưới ngưỡng budget của Angular (8kB/component).
@Component({
  selector: 'app-ly-do-chon-mua',
  templateUrl: './ly-do-chon-mua.html',
  styleUrl: './ly-do-chon-mua.scss'
})
export class LyDoChonMua {
  readonly tenSanPham = input.required<string>();
}
