import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Coupon } from '../../gio-hang/gio-hang/models/gio-hang.model';
import { CouponService } from '../../gio-hang/gio-hang/services/coupon.service';

// Banner PawVip + Flash Sale/coupon tách thành component riêng (giống deal-thang/blog-noi-bat) -
// tự lấy coupon đang hiệu lực của mình, không cần trang-chu truyền vào. Flash Sale trang chủ =
// coupon đang hiệu lực do Phần 3 quản lý, Phần 2 chỉ đọc để hiển thị (Pawsome_KhungDuAn.md mục 5.9)
// - chọn coupon % giảm cao nhất để làm nổi bật.
@Component({
  selector: 'app-banner-vip-uu-dai',
  imports: [RouterLink],
  templateUrl: './banner-vip-uu-dai.html',
  styleUrl: './banner-vip-uu-dai.scss'
})
export class BannerVipUuDai implements OnInit {
  private readonly couponService = inject(CouponService);

  readonly couponNoiBat = signal<Coupon | null>(null);

  ngOnInit(): void {
    this.couponService.layDangHieuLuc().subscribe(ds => this.couponNoiBat.set(this.chonCouponNoiBat(ds)));
  }

  private chonCouponNoiBat(list: Coupon[]): Coupon | null {
    if (list.length === 0) return null;

    const theoPhanTram = list.filter(c => c.loaiGiam === 'percent');
    const nguon = theoPhanTram.length > 0 ? theoPhanTram : list;
    return nguon.reduce((noiBat, c) => (c.giaTri > noiBat.giaTri ? c : noiBat), nguon[0]);
  }

  hienThiUuDai(cp: Coupon): string {
    return cp.loaiGiam === 'percent'
      ? `${cp.giaTri}% off`
      : `${cp.giaTri.toLocaleString('en-US')} VND off`;
  }
}
