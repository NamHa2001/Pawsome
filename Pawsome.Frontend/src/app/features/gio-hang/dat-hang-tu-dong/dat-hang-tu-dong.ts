import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AutoOrder, NHAN_TAN_SUAT, NHAN_TRANG_THAI_AUTO_ORDER, TanSuatDonTuDong } from '../gio-hang/models/gio-hang.model';
import { AutoOrderService } from '../dat-hang-tu-dong/services/auto-order.services';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

@Component({
  selector: 'app-dat-hang-tu-dong',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './dat-hang-tu-dong.html',
  styleUrl: './dat-hang-tu-dong.css'
})
export class DatHangTuDongComponent {
  private readonly autoOrderService = inject(AutoOrderService);

  readonly danhSach = signal<AutoOrder[]>([]);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);

  readonly dongDangSua = signal<number | null>(null);
  readonly soLuongSua = signal(1);
  readonly tanSuatSua = signal<TanSuatDonTuDong>('monthly');
  readonly dangXuLy = signal<number | null>(null);

  readonly nhanTanSuat = NHAN_TAN_SUAT;
  readonly nhanTrangThai = NHAN_TRANG_THAI_AUTO_ORDER;
  readonly tanSuatOptions: TanSuatDonTuDong[] = ['weekly', 'monthly', 'quarterly', 'yearly'];

  constructor() {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.autoOrderService.layDanhSach().subscribe({
      next: ds => {
        this.danhSach.set(ds);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Could not load your recurring orders.');
        this.dangTai.set(false);
      }
    });
  }

  moSua(donHang: AutoOrder): void {
    this.dongDangSua.set(donHang.autoOrderId);
    this.soLuongSua.set(donHang.soLuong);
    this.tanSuatSua.set(donHang.tanSuat);
  }

  huySua(): void {
    this.dongDangSua.set(null);
  }

  luuSua(id: number): void {
    this.dangXuLy.set(id);

    this.autoOrderService.capNhat(id, { soLuong: this.soLuongSua(), tanSuat: this.tanSuatSua() }).subscribe({
      next: res => {
        if (res.data) {
          this.capNhatTrongDanhSach(res.data);
        }
        this.dongDangSua.set(null);
        this.dangXuLy.set(null);
      },
      error: err => {
        this.loi.set(err?.error?.message ?? 'Update failed.');
        this.dangXuLy.set(null);
      }
    });
  }

  tamDung(id: number): void {
    this.dangXuLy.set(id);
    this.autoOrderService.tamDung(id).subscribe({
      next: () => { this.taiDanhSach(); this.dangXuLy.set(null); },
      error: () => this.dangXuLy.set(null)
    });
  }

  kichHoat(id: number): void {
    this.dangXuLy.set(id);
    this.autoOrderService.kichHoat(id).subscribe({
      next: () => { this.taiDanhSach(); this.dangXuLy.set(null); },
      error: () => this.dangXuLy.set(null)
    });
  }

  huy(id: number): void {
    if (!confirm('Cancel this recurring order?')) return;

    this.dangXuLy.set(id);
    this.autoOrderService.huy(id).subscribe({
      next: () => {
        this.danhSach.update(ds => ds.filter(d => d.autoOrderId !== id));
        this.dangXuLy.set(null);
      },
      error: () => this.dangXuLy.set(null)
    });
  }

  private capNhatTrongDanhSach(donMoi: AutoOrder): void {
    this.danhSach.update(ds => ds.map(d => d.autoOrderId === donMoi.autoOrderId ? donMoi : d));
  }
}