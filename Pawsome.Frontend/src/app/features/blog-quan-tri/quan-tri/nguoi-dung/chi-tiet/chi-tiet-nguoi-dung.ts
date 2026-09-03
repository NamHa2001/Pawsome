import { CommonModule } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AdminUser, PAWVIP_TIER_LABELS, ROLE_OPTIONS, daHetHan } from '../models/nguoi-dung.model';
import { AdminUserService } from '../services/admin-user.service';
import { AdminOrder } from '../../don-hang/models/don-hang.model';
import { AdminOrderService } from '../../don-hang/services/admin-order.service';
import { orderStatusCssClass, orderStatusLabel } from '../../../../don-hang/lich-su-don-hang/models/lich-su-don-hang.model';
import { DANH_SACH_GOI_PAWVIP } from '../../../../gio-hang/pawvip/models/pawvip-goi.model';

const PAGE_SIZE = 5;

@Component({
  selector: 'app-chi-tiet-nguoi-dung',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './chi-tiet-nguoi-dung.html',
  styleUrl: './chi-tiet-nguoi-dung.scss'
})
export class ChiTietNguoiDung {
  private readonly route = inject(ActivatedRoute);
  private readonly adminUserService = inject(AdminUserService);
  private readonly adminOrderService = inject(AdminOrderService);
  private readonly destroyRef = inject(DestroyRef);

  // Signal (không phải snapshot) - route này có thể bị Angular tái sử dụng cùng 1 instance
  // component khi điều hướng thẳng từ /quan-tri/nguoi-dung/5 sang /quan-tri/nguoi-dung/7
  // (cùng khớp 1 route config), nên phải subscribe route.paramMap để cập nhật lại, giống
  // cách blog-chi-tiet.ts/chi-tiet-san-pham.ts đã làm - không đọc 1 lần từ snapshot.
  readonly userId = signal(0);

  readonly user = signal<AdminUser | null>(null);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);
  readonly dangCapNhat = signal(false);
  readonly loiHanhDong = signal<string | null>(null);

  readonly donHang = signal<AdminOrder[]>([]);
  readonly tongSoDon = signal(0);
  readonly trangDon = signal(1);
  readonly tongTrangDon = signal(0);
  readonly dangTaiDon = signal(true);
  readonly loiTaiDon = signal<string | null>(null);
  readonly moRongDon = signal<number | null>(null);

  readonly cacVaiTro = ROLE_OPTIONS;
  readonly nhanGoiPawVip = PAWVIP_TIER_LABELS;
  orderStatusLabel = orderStatusLabel;
  orderStatusCssClass = orderStatusCssClass;
  readonly cacTrangDon = computed(() => Array.from({ length: this.tongTrangDon() }, (_, i) => i + 1));

  // Tra cứu đặc quyền của gói PawVip hiện tại của khách (khớp đúng danh sách đang hiện ở trang
  // PawVip khách hàng) - hiện trên trang chi tiết để admin thấy khách đang được hưởng gì, không
  // chỉ mỗi tên gói.
  readonly goiPawVipDaChon = computed(() => {
    const tier = this.user()?.pawVipTier;
    return tier ? DANH_SACH_GOI_PAWVIP.find(g => g.id === tier) : undefined;
  });

  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      this.userId.set(Number(params.get('id')));
      this.trangDon.set(1);
      this.taiNguoiDung();
      this.taiDonHang();
    });
  }

  readonly daHetHan = daHetHan;

  private taiNguoiDung(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.adminUserService.layChiTiet(this.userId()).subscribe({
      next: u => {
        this.user.set(u);
        this.dangTai.set(false);
      },
      error: () => {
        this.loiTai.set('Failed to load this account.');
        this.dangTai.set(false);
      }
    });
  }

  private taiDonHang(): void {
    this.dangTaiDon.set(true);
    this.loiTaiDon.set(null);

    this.adminOrderService.layDanhSach({
      userId: this.userId(),
      page: this.trangDon(),
      pageSize: PAGE_SIZE
    }).subscribe({
      next: ket => {
        this.donHang.set(ket.items);
        this.tongSoDon.set(ket.totalCount);
        this.tongTrangDon.set(ket.totalPages);
        this.trangDon.set(ket.pageNumber);
        this.dangTaiDon.set(false);
      },
      error: () => {
        this.loiTaiDon.set('Failed to load order history.');
        this.dangTaiDon.set(false);
      }
    });
  }

  doiTrangDon(trang: number): void {
    if (trang < 1 || trang > this.tongTrangDon()) return;
    this.trangDon.set(trang);
    this.taiDonHang();
  }

  toggleMoRongDon(orderId: number): void {
    this.moRongDon.set(this.moRongDon() === orderId ? null : orderId);
  }

  toggleKhoa(): void {
    const u = this.user();
    if (!u || this.dangCapNhat()) return;

    this.loiHanhDong.set(null);
    this.dangCapNhat.set(true);

    const dangKhoa = u.trangThai === 'locked';
    const goi = dangKhoa
      ? this.adminUserService.moKhoa(u.userId)
      : this.adminUserService.khoa(u.userId);

    goi.subscribe({
      next: () => {
        this.user.set({ ...u, trangThai: dangKhoa ? 'active' : 'locked' });
        this.dangCapNhat.set(false);
      },
      error: err => {
        this.loiHanhDong.set(err?.error?.message ?? 'Failed to update account status.');
        this.dangCapNhat.set(false);
      }
    });
  }

  doiVaiTro(tenVaiTroMoi: string): void {
    const u = this.user();
    const vaiTroMoi = this.cacVaiTro.find(r => r.ten === tenVaiTroMoi);
    if (!u || !vaiTroMoi || vaiTroMoi.ten === u.role || this.dangCapNhat()) return;

    this.loiHanhDong.set(null);
    this.dangCapNhat.set(true);

    this.adminUserService.doiVaiTro(u.userId, vaiTroMoi.id).subscribe({
      next: () => {
        this.user.set({ ...u, role: vaiTroMoi.ten });
        this.dangCapNhat.set(false);
      },
      error: err => {
        this.loiHanhDong.set(err?.error?.message ?? 'Failed to update role.');
        this.dangCapNhat.set(false);
      }
    });
  }
}
