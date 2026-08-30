import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { PawVipService } from './services/pawvip.service';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { DANH_SACH_GOI_PAWVIP, MaGoiPawVip } from './models/pawvip-goi.model';

@Component({
  selector: 'app-pawvip',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './pawvip.html',
  styleUrl: './pawvip.css'
})
export class PawVipComponent {
  private readonly pawVipService = inject(PawVipService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  private readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  // 3 plans: Basic (5%), Advanced (10%), VIP (20%)
  readonly goiPawVipList = DANH_SACH_GOI_PAWVIP;
  readonly goiPawVipDangChon = signal<MaGoiPawVip>('vip');
  readonly goiPawVipHienTai = computed(() =>
    this.goiPawVipList.find(g => g.id === this.goiPawVipDangChon()) ?? this.goiPawVipList[this.goiPawVipList.length - 1]
  );

  // Gói đang thực sự active của tài khoản (khác goiPawVipDangChon - cái đó chỉ là gói đang
  // xem/preview). null nghĩa là chưa là thành viên PawVip.
  readonly tierDangKichHoat = signal<MaGoiPawVip | null>(null);
  readonly hetHanHienTai = signal<string | null>(null);

  chonGoiPawVip(id: MaGoiPawVip): void {
    this.goiPawVipDangChon.set(id);
  }

  constructor() {
    this.taiTrangThaiPawVip();
  }

  private taiTrangThaiPawVip(): void {
    if (!this.daDangNhap()) return;

    this.pawVipService.layTrangThai().subscribe({
      next: res => {
        const tier = res.data?.tier as MaGoiPawVip | null | undefined;
        if (tier) {
          this.tierDangKichHoat.set(tier);
          this.goiPawVipDangChon.set(tier);
          this.hetHanHienTai.set(res.data?.hetHan ?? null);
        }
      },
      error: () => {
        // Không chặn xem trang nếu chỉ lỗi lấy trạng thái - coi như chưa là thành viên
      }
    });
  }

  // Việc chọn phương thức thanh toán & trả tiền tách sang trang riêng (pawvip-thanh-toan),
  // giống cấu trúc 2 bước gio-hang -> thanh-toan của luồng đơn hàng thường - trang này chỉ
  // lo chọn gói, chuyển tier đã chọn qua query param.
  tiepTucThanhToan(): void {
    if (!this.daDangNhap()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    this.router.navigate(['/gio-hang/pawvip/thanh-toan'], { queryParams: { tier: this.goiPawVipDangChon() } });
  }
}
