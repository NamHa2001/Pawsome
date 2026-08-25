import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  ORDER_STATUS_FILTERS,
  orderStatusCssClass,
  orderStatusLabel
} from '../../../don-hang/lich-su-don-hang/models/lich-su-don-hang.model';
import { AdminOrder } from './models/don-hang.model';
import { AdminOrderService } from './services/admin-order.service';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-quan-tri-don-hang',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './don-hang.html',
  styleUrl: './don-hang.scss'
})
export class QuanTriDonHang {
  private readonly adminOrderService = inject(AdminOrderService);

  readonly danhSach = signal<AdminOrder[]>([]);
  readonly tongSo = signal(0);
  readonly trang = signal(1);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);
  readonly loiHanhDong = signal<string | null>(null);
  readonly dangCapNhat = signal<ReadonlySet<number>>(new Set());
  readonly moRong = signal<number | null>(null);
  readonly trangThaiLoc = signal('');

  readonly boLocTrangThai = ORDER_STATUS_FILTERS;
  readonly tuyChonTrangThai = ORDER_STATUS_FILTERS.filter(o => o.value !== '');

  readonly cacTrang = computed(() => Array.from({ length: this.tongTrang() }, (_, i) => i + 1));

  constructor() {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.adminOrderService.layDanhSach({
      trangThai: this.trangThaiLoc() || undefined,
      page: this.trang(),
      pageSize: PAGE_SIZE
    }).subscribe({
      next: ket => {
        this.danhSach.set(ket.items);
        this.tongSo.set(ket.totalCount);
        this.tongTrang.set(ket.totalPages);
        this.trang.set(ket.pageNumber);
        this.dangTai.set(false);
      },
      error: () => {
        this.loiTai.set('Failed to load orders.');
        this.dangTai.set(false);
      }
    });
  }

  doiBoLoc(trangThai: string): void {
    this.trangThaiLoc.set(trangThai);
    this.trang.set(1);
    this.taiDanhSach();
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.trang.set(trang);
    this.taiDanhSach();
  }

  toggleMoRong(orderId: number): void {
    this.moRong.set(this.moRong() === orderId ? null : orderId);
  }

  dangCapNhatDong(orderId: number): boolean {
    return this.dangCapNhat().has(orderId);
  }

  capNhatTrangThai(order: AdminOrder, trangThaiMoi: string): void {
    if (trangThaiMoi === order.trangThai || this.dangCapNhatDong(order.orderId)) return;

    this.loiHanhDong.set(null);
    this.dangCapNhat.update(ds => new Set(ds).add(order.orderId));

    this.adminOrderService.capNhatTrangThai(order.orderId, trangThaiMoi).subscribe({
      next: ketQua => {
        this.danhSach.update(ds => ds.map(o => o.orderId === ketQua.orderId ? ketQua : o));
        this.ketThucCapNhat(order.orderId);
      },
      error: err => {
        this.loiHanhDong.set(err?.error?.message ?? 'Failed to update order status.');
        this.ketThucCapNhat(order.orderId);
      }
    });
  }

  orderStatusLabel = orderStatusLabel;
  orderStatusCssClass = orderStatusCssClass;

  private ketThucCapNhat(orderId: number): void {
    this.dangCapNhat.update(ds => {
      const moi = new Set(ds);
      moi.delete(orderId);
      return moi;
    });
  }
}
