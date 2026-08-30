import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminUser, PAWVIP_TIER_LABELS, ROLE_OPTIONS } from './models/nguoi-dung.model';
import { AdminUserService } from './services/admin-user.service';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-quan-tri-nguoi-dung',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './nguoi-dung.html',
  styleUrl: './nguoi-dung.scss'
})
export class QuanTriNguoiDung {
  private readonly adminUserService = inject(AdminUserService);

  readonly danhSach = signal<AdminUser[]>([]);
  readonly tongSo = signal(0);
  readonly trang = signal(1);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);
  readonly loiHanhDong = signal<string | null>(null);
  readonly dangCapNhat = signal<ReadonlySet<number>>(new Set());
  readonly tuKhoa = signal('');
  readonly trangThaiLoc = signal('');

  readonly cacVaiTro = ROLE_OPTIONS;
  readonly cacTrang = computed(() => Array.from({ length: this.tongTrang() }, (_, i) => i + 1));
  readonly nhanGoiPawVip = PAWVIP_TIER_LABELS;

  constructor() {
    this.taiDanhSach();
  }

  // pawvip_het_han lưu ngày hết hạn theo NĂM (xem PawVipTiers.ConHieuLuc ở backend) - admin
  // vẫn cần thấy các gói đã hết hạn để tra soát, nên so sánh ngay ở đây thay vì lọc từ backend
  // như hồ sơ khách tự xem, chỉ để đánh dấu "Expired" cho rõ chứ không ẩn đi.
  daHetHan(hetHan: string | null): boolean {
    if (!hetHan) return false;
    return new Date(hetHan) < new Date(new Date().toDateString());
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.adminUserService.layDanhSach({
      keyword: this.tuKhoa().trim() || undefined,
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
        this.loiTai.set('Failed to load users.');
        this.dangTai.set(false);
      }
    });
  }

  timKiem(): void {
    this.trang.set(1);
    this.taiDanhSach();
  }

  doiBoLocTrangThai(trangThai: string): void {
    this.trangThaiLoc.set(trangThai);
    this.trang.set(1);
    this.taiDanhSach();
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.trang.set(trang);
    this.taiDanhSach();
  }

  dangCapNhatDong(userId: number): boolean {
    return this.dangCapNhat().has(userId);
  }

  toggleKhoa(user: AdminUser): void {
    if (this.dangCapNhatDong(user.userId)) return;

    this.loiHanhDong.set(null);
    this.batDauCapNhat(user.userId);

    const dangKhoa = user.trangThai === 'locked';
    const goi = dangKhoa
      ? this.adminUserService.moKhoa(user.userId)
      : this.adminUserService.khoa(user.userId);

    goi.subscribe({
      next: () => {
        this.danhSach.update(ds => ds.map(u =>
          u.userId === user.userId ? { ...u, trangThai: dangKhoa ? 'active' : 'locked' } : u
        ));
        this.ketThucCapNhat(user.userId);
      },
      error: err => {
        this.loiHanhDong.set(err?.error?.message ?? 'Failed to update account status.');
        this.ketThucCapNhat(user.userId);
      }
    });
  }

  doiVaiTro(user: AdminUser, tenVaiTroMoi: string): void {
    const vaiTroMoi = this.cacVaiTro.find(r => r.ten === tenVaiTroMoi);
    if (!vaiTroMoi || vaiTroMoi.ten === user.role || this.dangCapNhatDong(user.userId)) return;

    this.loiHanhDong.set(null);
    this.batDauCapNhat(user.userId);

    this.adminUserService.doiVaiTro(user.userId, vaiTroMoi.id).subscribe({
      next: () => {
        this.danhSach.update(ds => ds.map(u =>
          u.userId === user.userId ? { ...u, role: vaiTroMoi.ten } : u
        ));
        this.ketThucCapNhat(user.userId);
      },
      error: err => {
        this.loiHanhDong.set(err?.error?.message ?? 'Failed to update role.');
        this.ketThucCapNhat(user.userId);
      }
    });
  }

  private batDauCapNhat(userId: number): void {
    this.dangCapNhat.update(ds => new Set(ds).add(userId));
  }

  private ketThucCapNhat(userId: number): void {
    this.dangCapNhat.update(ds => {
      const moi = new Set(ds);
      moi.delete(userId);
      return moi;
    });
  }
}
