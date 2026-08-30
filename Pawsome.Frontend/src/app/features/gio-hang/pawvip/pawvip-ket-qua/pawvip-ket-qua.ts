import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Footer } from '../../../../shared/components/footer/footer';
import { Header } from '../../../../shared/components/header/header';
import { PawVipService } from '../services/pawvip.service';

type KetQua = 'dang_xac_nhan' | 'thanh_cong' | 'that_bai' | 'khong_ro';

const SO_GIAY_DEM_NGUOC = 6;
const SO_LAN_THU_LAI_TOI_DA = 3;
const KHOANG_CACH_THU_LAI_MS = 1500;

@Component({
  selector: 'app-pawvip-ket-qua',
  standalone: true,
  imports: [CommonModule, Header, Footer],
  templateUrl: './pawvip-ket-qua.html',
  styleUrl: './pawvip-ket-qua.css'
})
export class PawVipKetQuaComponent implements OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly pawVipService = inject(PawVipService);

  readonly ketQua = signal<KetQua>('dang_xac_nhan');
  readonly tier = signal<string | null>(null);
  readonly demNguoc = signal(SO_GIAY_DEM_NGUOC);

  private demNguocTimer?: ReturnType<typeof setInterval>;
  private thuLaiTimer?: ReturnType<typeof setTimeout>;

  constructor() {
    const params = this.route.snapshot.queryParamMap;
    const id = this.trichXuatPawVipPaymentId(params);

    if (id === null) {
      this.ketQua.set('khong_ro');
      this.batDauDemNguoc();
      return;
    }

    this.xacNhanTrangThai(id, 0);
  }

  ngOnDestroy(): void {
    if (this.demNguocTimer) clearInterval(this.demNguocTimer);
    if (this.thuLaiTimer) clearTimeout(this.thuLaiTimer);
  }

  goNgayVePawVip(): void {
    if (this.demNguocTimer) clearInterval(this.demNguocTimer);
    this.router.navigateByUrl('/gio-hang/pawvip');
  }

  // MoMo trả về ?orderId=pv{id}-{timestamp}, VNPay trả về ?vnp_TxnRef=pv{id}-{timestamp} -
  // tiền tố "pv" do PawVipPaymentService tự đặt lúc tạo giao dịch (khác định dạng số thuần
  // của đơn hàng thường), tách lấy id để hỏi đúng bảng pawvip_payments.
  private trichXuatPawVipPaymentId(params: import('@angular/router').ParamMap): number | null {
    const vnpTxnRef = params.get('vnp_TxnRef');
    const momoOrderId = params.get('orderId');
    const raw = vnpTxnRef ?? momoOrderId;
    if (!raw || !raw.startsWith('pv')) return null;

    const id = parseInt(raw.slice(2).split('-')[0], 10);
    return Number.isNaN(id) ? null : id;
  }

  private xacNhanTrangThai(id: number, lanThu: number): void {
    this.pawVipService.getPaymentStatus(id).subscribe({
      next: res => {
        const data = res.data;
        if (!data) {
          this.ketQua.set('khong_ro');
          this.batDauDemNguoc();
          return;
        }

        this.tier.set(data.tier);

        if (data.trangThai === 'thanh_cong') {
          this.ketQua.set('thanh_cong');
          this.batDauDemNguoc();
        } else if (data.trangThai === 'that_bai') {
          this.ketQua.set('that_bai');
          this.batDauDemNguoc();
        } else if (lanThu < SO_LAN_THU_LAI_TOI_DA) {
          this.thuLaiTimer = setTimeout(() => this.xacNhanTrangThai(id, lanThu + 1), KHOANG_CACH_THU_LAI_MS);
        } else {
          this.ketQua.set('khong_ro');
          this.batDauDemNguoc();
        }
      },
      error: () => {
        this.ketQua.set('khong_ro');
        this.batDauDemNguoc();
      }
    });
  }

  private batDauDemNguoc(): void {
    this.demNguoc.set(SO_GIAY_DEM_NGUOC);
    this.demNguocTimer = setInterval(() => {
      const con = this.demNguoc() - 1;
      if (con <= 0) {
        clearInterval(this.demNguocTimer);
        this.router.navigateByUrl('/gio-hang/pawvip');
      } else {
        this.demNguoc.set(con);
      }
    }, 1000);
  }
}
