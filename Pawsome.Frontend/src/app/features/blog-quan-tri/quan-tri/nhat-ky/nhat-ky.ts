import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuditLog } from './models/nhat-ky.model';
import { AuditLogService } from './services/audit-log.service';

const PAGE_SIZE = 20;

@Component({
  selector: 'app-quan-tri-nhat-ky',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nhat-ky.html',
  styleUrl: './nhat-ky.scss'
})
export class QuanTriNhatKy {
  private readonly auditLogService = inject(AuditLogService);

  readonly danhSach = signal<AuditLog[]>([]);
  readonly tongSo = signal(0);
  readonly trang = signal(1);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);

  hanhDongLoc = '';
  tuNgayLoc = '';
  denNgayLoc = '';

  readonly cacTrang = computed(() => Array.from({ length: this.tongTrang() }, (_, i) => i + 1));

  constructor() {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.auditLogService.layDanhSach({
      hanhDong: this.hanhDongLoc.trim() || undefined,
      tuNgay: this.tuNgayLoc || undefined,
      denNgay: this.denNgayLoc || undefined,
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
        this.loiTai.set('Failed to load audit logs.');
        this.dangTai.set(false);
      }
    });
  }

  apDungBoLoc(): void {
    this.trang.set(1);
    this.taiDanhSach();
  }

  xoaBoLoc(): void {
    this.hanhDongLoc = '';
    this.tuNgayLoc = '';
    this.denNgayLoc = '';
    this.trang.set(1);
    this.taiDanhSach();
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.trang.set(trang);
    this.taiDanhSach();
  }
}
