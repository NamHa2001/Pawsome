import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { Order } from '../thanh-toan/models/thanh-toan.model';
import { OrderService } from '../thanh-toan/services/order.service';

type KetQua = 'dang_xac_nhan' | 'thanh_cong' | 'that_bai' | 'khong_ro';

const TRANG_THAI_DA_THANH_TOAN = new Set(['dang_xu_ly', 'da_giao_van', 'da_giao']);

const SO_GIAY_DEM_NGUOC = 6;
const SO_LAN_THU_LAI_TOI_DA = 3;
const KHOANG_CACH_THU_LAI_MS = 1500;

@Component({
  selector: 'app-ket-qua-thanh-toan',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer],
  templateUrl: './ket-qua-thanh-toan.html',
  styleUrl: './ket-qua-thanh-toan.css'
})
export class KetQuaThanhToanComponent implements OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);

  readonly ketQua = signal<KetQua>('dang_xac_nhan');
  readonly order = signal<Order | null>(null);
  readonly orderId = signal<number | null>(null);
  readonly demNguoc = signal(SO_GIAY_DEM_NGUOC);

  private demNguocTimer?: ReturnType<typeof setInterval>;
  private thuLaiTimer?: ReturnType<typeof setTimeout>;

  constructor() {
    const params = this.route.snapshot.queryParamMap;

    const oid = this.trichXuatOrderId(params);
    const doanTuGateway = this.doanKetQuaTuGateway(params);

    if (oid === null) {
      this.ketQua.set(doanTuGateway === true ? 'thanh_cong' : doanTuGateway === false ? 'that_bai' : 'khong_ro');
      this.batDauDemNguoc();
      return;
    }

    this.orderId.set(oid);
    this.xacNhanTrangThaiDon(oid, doanTuGateway, 0);
  }

  ngOnDestroy(): void {
    if (this.demNguocTimer) clearInterval(this.demNguocTimer);
    if (this.thuLaiTimer) clearTimeout(this.thuLaiTimer);
  }

  goNgayVeTrangChu(): void {
    if (this.demNguocTimer) clearInterval(this.demNguocTimer);
    this.router.navigateByUrl('/');
  }

  private trichXuatOrderId(params: import('@angular/router').ParamMap): number | null {
    const vnpTxnRef = params.get('vnp_TxnRef');
    const momoOrderId = params.get('orderId');
    const raw = vnpTxnRef ?? momoOrderId;
    if (!raw) return null;

    const id = parseInt(raw.split('-')[0], 10);
    return Number.isNaN(id) ? null : id;
  }

  private doanKetQuaTuGateway(params: import('@angular/router').ParamMap): boolean | null {
    if (params.has('vnp_ResponseCode')) return params.get('vnp_ResponseCode') === '00';
    if (params.has('resultCode')) return params.get('resultCode') === '0';
    return null;
  }

  private xacNhanTrangThaiDon(orderId: number, doanTuGateway: boolean | null, lanThu: number): void {
    this.orderService.getById(orderId).subscribe({
      next: order => {
        this.order.set(order);

        if (TRANG_THAI_DA_THANH_TOAN.has(order.trangThai)) {
          this.ketQua.set('thanh_cong');
          this.batDauDemNguoc();
        } else if (order.trangThai === 'da_huy') {
          this.ketQua.set('that_bai');
          this.batDauDemNguoc();
        } else if (order.trangThai === 'cho_xu_ly' && lanThu < SO_LAN_THU_LAI_TOI_DA) {
          this.thuLaiTimer = setTimeout(
            () => this.xacNhanTrangThaiDon(orderId, doanTuGateway, lanThu + 1),
            KHOANG_CACH_THU_LAI_MS
          );
        } else {
          this.ketQua.set(doanTuGateway === false ? 'that_bai' : 'khong_ro');
          this.batDauDemNguoc();
        }
      },
      error: () => {
        this.ketQua.set(doanTuGateway === true ? 'thanh_cong' : doanTuGateway === false ? 'that_bai' : 'khong_ro');
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
        this.router.navigateByUrl('/');
      } else {
        this.demNguoc.set(con);
      }
    }, 1000);
  }
}