import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Cart } from '../gio-hang/models/gio-hang.model';
import { CartService } from '../gio-hang/services/cart.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

type MaGoiPawVip = 'thuong' | 'nang-cao' | 'vip';

interface GoiPawVip {
  id: MaGoiPawVip;
  ten: string;
  phanTramGiam: number;
  giaNam: number; // TODO: update with the real price for each plan once available
  noiBat: boolean;
  quyenLoi: string[];
}

const DANH_SACH_GOI_PAWVIP: GoiPawVip[] = [
  {
    id: 'thuong',
    ten: 'Basic Plan',
    phanTramGiam: 0.05,
    giaNam: 594000,
    noiBat: false,
    quyenLoi: ['5% off every order', 'Basic priority support']
  },
  {
    id: 'nang-cao',
    ten: 'Advanced Plan',
    phanTramGiam: 0.1,
    giaNam: 1188000,
    noiBat: false,
    quyenLoi: ['10% off every order', 'Faster priority support', 'Free standard shipping']
  },
  {
    id: 'vip',
    ten: 'VIP Plan',
    phanTramGiam: 0.2,
    giaNam: 2376000,
    noiBat: true,
    quyenLoi: ['20% off every order', 'Top-priority support', 'Free shipping on every order', 'Pet birthday gift']
  }
];

@Component({
  selector: 'app-pawvip',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './pawvip.html',
  styleUrl: './pawvip.css'
})
export class PawVipComponent {
  private readonly cartService = inject(CartService);

  readonly gioHang = signal<Cart | null>(null);
  readonly dangTai = signal(true);

  // 3 plans: Basic (5%), Advanced (10%), VIP (20%)
  readonly goiPawVipList = DANH_SACH_GOI_PAWVIP;
  readonly goiPawVipDangChon = signal<MaGoiPawVip>('vip');
  readonly goiPawVipHienTai = computed(() =>
    this.goiPawVipList.find(g => g.id === this.goiPawVipDangChon()) ?? this.goiPawVipList[this.goiPawVipList.length - 1]
  );

  readonly soLuongSanPham = computed(() => this.gioHang()?.items.length ?? 0);
  readonly tienHangHienTai = computed(() => this.gioHang()?.tienHang ?? 0);
  readonly tietKiemPawVip = computed(() => this.tienHangHienTai() * this.goiPawVipHienTai().phanTramGiam);
  readonly tongTienPawVip = computed(() => this.tienHangHienTai() - this.tietKiemPawVip());

  chonGoiPawVip(id: MaGoiPawVip): void {
    this.goiPawVipDangChon.set(id);
  }

  constructor() {
    this.taiGioHang();
  }

  private taiGioHang(): void {
    this.dangTai.set(true);
    this.cartService.layGioHang().subscribe({
      next: res => {
        this.gioHang.set(res.data ?? null);
        this.dangTai.set(false);
      },
      error: () => {
        this.gioHang.set(null);
        this.dangTai.set(false);
      }
    });
  }

  kichHoatPawVip(): void {

  }
}