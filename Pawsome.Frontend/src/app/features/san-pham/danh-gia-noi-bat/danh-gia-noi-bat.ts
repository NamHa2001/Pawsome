import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DanhGia } from '../models/san-pham.model';
import { ReviewService } from '../services/review.service';

// "What Our Happy Customers Say" tách thành component riêng (giống banner-noi-bat/deal-thang) -
// tự lấy dữ liệu đánh giá tiêu biểu của mình, không cần trang-chu truyền vào - vừa gọn trang-chu.ts,
// vừa giảm CSS trang-chu.scss xuống dưới ngưỡng budget của Angular.
@Component({
  selector: 'app-danh-gia-noi-bat',
  templateUrl: './danh-gia-noi-bat.html',
  styleUrl: './danh-gia-noi-bat.scss'
})
export class DanhGiaNoiBat implements OnInit {
  private readonly reviewService = inject(ReviewService);

  readonly danhGiaNoiBat = signal<DanhGia[]>([]);
  readonly saoArr = [1, 2, 3, 4, 5];

  // Hiển thị 2 đánh giá/lần, bấm mũi tên trái/phải để xem cặp tiếp theo (Reviews đã tải sẵn cả
  // danhGiaNoiBat() từ backend - soLuong=6 - nên chuyển trang chỉ cần cắt mảng, không gọi lại API).
  private readonly SO_DANH_GIA_MOI_TRANG = 2;
  readonly trangDanhGiaNoiBat = signal(0);

  readonly tongTrangDanhGiaNoiBat = computed(() =>
    Math.ceil(this.danhGiaNoiBat().length / this.SO_DANH_GIA_MOI_TRANG)
  );

  readonly danhGiaHienThi = computed(() => {
    const batDau = this.trangDanhGiaNoiBat() * this.SO_DANH_GIA_MOI_TRANG;
    return this.danhGiaNoiBat().slice(batDau, batDau + this.SO_DANH_GIA_MOI_TRANG);
  });

  ngOnInit(): void {
    this.reviewService.getNoiBat(6).subscribe(ds => this.danhGiaNoiBat.set(ds));
  }

  doiTrangNoiBat(buoc: number): void {
    const trangCuoi = this.tongTrangDanhGiaNoiBat() - 1;
    this.trangDanhGiaNoiBat.update(t => Math.min(Math.max(t + buoc, 0), trangCuoi));
  }
}
