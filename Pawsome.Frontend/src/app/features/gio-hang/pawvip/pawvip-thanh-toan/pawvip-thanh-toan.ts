import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { PawVipService } from '../services/pawvip.service';
import { DANH_SACH_GOI_PAWVIP } from '../models/pawvip-goi.model';
import { PaymentMethod } from '../../../don-hang/thanh-toan/models/thanh-toan.model';
import { TokenService } from '../../../../core/models/token.service';
import { ChatAi } from '../../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../../shared/components/footer/footer';
import { Header } from '../../../../shared/components/header/header';

@Component({
  selector: 'app-pawvip-thanh-toan',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './pawvip-thanh-toan.html',
  styleUrl: './pawvip-thanh-toan.css'
})
export class PawVipThanhToanComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly pawVipService = inject(PawVipService);
  private readonly tokenService = inject(TokenService);

  private readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });

  readonly dangXuLy = signal(false);
  readonly thongBaoLoi = signal<string | null>(null);
  readonly selectedPaymentMethod = signal<PaymentMethod>('momo');

  // Gói khách đã chọn ở trang trước, truyền qua query param ?tier=... - trang này chỉ review
  // lại + thu phương thức thanh toán, không cho đổi gói (đổi thì quay lại trang chọn gói).
  // Đây là mua gói thuê bao (subscription), không phải mua hàng theo giỏ - tóm tắt đơn ở đây
  // chỉ gồm đúng 1 dòng "giá gói/năm", không liên quan gì tới giỏ hàng hiện tại của khách,
  // giống trang thanh toán của các dịch vụ subscription thật (Claude, Netflix...).
  readonly goiDaChon = DANH_SACH_GOI_PAWVIP.find(g => g.id === this.route.snapshot.queryParamMap.get('tier'));

  constructor() {
    // Chưa đăng nhập, chưa chọn gói ở trang trước, hoặc gói trong URL không hợp lệ - quay lại
    // trang chọn gói thay vì để lỗi giữa chừng ở đây (khách có thể tự gõ URL trực tiếp).
    if (this.nguoiDungHienTai() === null) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }
    if (!this.goiDaChon) {
      this.router.navigateByUrl('/gio-hang/pawvip');
      return;
    }

    this.kiemTraDaKichHoatChuaHay();
  }

  selectPaymentMethod(method: PaymentMethod): void {
    this.selectedPaymentMethod.set(method);
  }

  // Gói trong URL trùng gói đang active thật của tài khoản - không có gì để thanh toán, quay
  // lại trang chọn gói (chặn khách bị "mua lại" gói đang có bằng cách sửa tay query param).
  private kiemTraDaKichHoatChuaHay(): void {
    this.pawVipService.layTrangThai().subscribe({
      next: res => {
        if (res.data?.tier === this.goiDaChon?.id) {
          this.router.navigateByUrl('/gio-hang/pawvip');
        }
      },
      error: () => {}
    });
  }

  xacNhanThanhToan(): void {
    if (!this.goiDaChon) return;

    this.dangXuLy.set(true);
    this.thongBaoLoi.set(null);

    const tier = this.goiDaChon.id;
    const create = this.selectedPaymentMethod() === 'momo'
      ? this.pawVipService.createMoMoPayment(tier)
      : this.pawVipService.createVnPayPayment(tier);

    create.subscribe({
      next: result => {
        window.location.href = result.payUrl;
      },
      error: err => {
        this.dangXuLy.set(false);
        this.thongBaoLoi.set(err?.error?.message ?? 'Failed to start payment. Please try again.');
      }
    });
  }
}
