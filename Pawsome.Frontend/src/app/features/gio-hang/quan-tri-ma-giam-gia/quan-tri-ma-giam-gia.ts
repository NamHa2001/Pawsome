import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Coupon, CreateCoupon, LoaiGiamCoupon, UpdateCoupon } from '../gio-hang/models/gio-hang.model';
import { CouponService } from '../gio-hang/services/coupon.service';

interface FormCouponState {
  maCode: string;
  loaiGiam: LoaiGiamCoupon;
  giaTri: number;
  soLuong: number | null;
  ngayBatDau: string | null;
  ngayKetThuc: string | null;
}

const FORM_MAC_DINH: FormCouponState = {
  maCode: '',
  loaiGiam: 'percent',
  giaTri: 0,
  soLuong: null,
  ngayBatDau: null,
  ngayKetThuc: null
};

@Component({
  selector: 'app-quan-tri-ma-giam-gia',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './quan-tri-ma-giam-gia.html',
  styleUrl: './quan-tri-ma-giam-gia.css'
})
export class QuanTriMaGiamGiaComponent {
  private readonly couponService = inject(CouponService);

  readonly danhSach = signal<Coupon[]>([]);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);
  readonly thongBao = signal<string | null>(null);

  // Add form
  readonly dangMoFormThem = signal(false);
  readonly formThem = signal<FormCouponState>({ ...FORM_MAC_DINH });
  readonly loiForm = signal<string | null>(null);
  readonly dangLuu = signal(false);

  // Edit form
  readonly idDangSua = signal<number | null>(null);
  readonly formSua = signal<FormCouponState>({ ...FORM_MAC_DINH });

  readonly dangXoa = signal<number | null>(null);

  constructor() {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.couponService.layTatCa().subscribe({
      next: ds => {
        this.danhSach.set(ds);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Could not load the coupon list.');
        this.dangTai.set(false);
      }
    });
  }

  // ── Create ──
  moFormThem(): void {
    this.formThem.set({ ...FORM_MAC_DINH });
    this.loiForm.set(null);
    this.dangMoFormThem.set(true);
  }

  dongFormThem(): void {
    this.dangMoFormThem.set(false);
    this.loiForm.set(null);
  }

  luuThem(): void {
    const loi = this.kiemTraForm(this.formThem());
    if (loi) {
      this.loiForm.set(loi);
      return;
    }

    const f = this.formThem();
    const dto: CreateCoupon = {
      maCode: f.maCode.trim().toUpperCase(),
      loaiGiam: f.loaiGiam,
      giaTri: f.giaTri,
      ngayBatDau: f.ngayBatDau,
      ngayKetThuc: f.ngayKetThuc,
      soLuong: f.soLuong
    };

    this.dangLuu.set(true);
    this.couponService.tao(dto).subscribe({
      next: res => {
        this.dangLuu.set(false);
        this.dangMoFormThem.set(false);
        this.thongBao.set(res.message ?? 'Coupon created successfully.');
        this.taiDanhSach();
      },
      error: err => {
        this.dangLuu.set(false);
        this.loiForm.set(err?.error?.message ?? 'Failed to create coupon.');
      }
    });
  }

  // ── Edit ───
  moFormSua(c: Coupon): void {
    this.idDangSua.set(c.couponId);
    this.formSua.set({
      maCode: c.maCode,
      loaiGiam: c.loaiGiam as LoaiGiamCoupon,
      giaTri: c.giaTri,
      soLuong: c.soLuong,
      ngayBatDau: c.ngayBatDau,
      ngayKetThuc: c.ngayKetThuc
    });
    this.loiForm.set(null);
  }

  huySua(): void {
    this.idDangSua.set(null);
    this.loiForm.set(null);
  }

  luuSua(id: number): void {
    const loi = this.kiemTraForm(this.formSua());
    if (loi) {
      this.loiForm.set(loi);
      return;
    }

    const f = this.formSua();
    const dto: UpdateCoupon = {
      loaiGiam: f.loaiGiam,
      giaTri: f.giaTri,
      ngayBatDau: f.ngayBatDau,
      ngayKetThuc: f.ngayKetThuc,
      soLuong: f.soLuong
    };

    this.dangLuu.set(true);
    this.couponService.capNhat(id, dto).subscribe({
      next: res => {
        this.dangLuu.set(false);
        this.idDangSua.set(null);
        this.thongBao.set(res.message ?? 'Coupon updated successfully.');
        this.taiDanhSach();
      },
      error: err => {
        this.dangLuu.set(false);
        this.loiForm.set(err?.error?.message ?? 'Update failed.');
      }
    });
  }

  // ── Delete ───
  xoa(c: Coupon): void {
    if (!confirm(`Delete coupon "${c.maCode}"?`)) return;

    this.dangXoa.set(c.couponId);
    this.couponService.xoa(c.couponId).subscribe({
      next: () => {
        this.danhSach.update(ds => ds.filter(x => x.couponId !== c.couponId));
        this.thongBao.set('Coupon deleted.');
        this.dangXoa.set(null);
      },
      error: err => {
        this.loi.set(err?.error?.message ?? 'Failed to delete coupon.');
        this.dangXoa.set(null);
      }
    });
  }

  // ── Shared validation for add & edit forms ───
  private kiemTraForm(f: FormCouponState): string | null {
    if (!f.maCode || !f.maCode.trim()) return 'Please enter a coupon code.';
    if (f.giaTri === null || f.giaTri === undefined || f.giaTri <= 0) return 'Discount value must be greater than 0.';
    if (f.loaiGiam === 'percent' && f.giaTri > 100) return 'Percentage discount cannot exceed 100%.';
    if (f.ngayBatDau && f.ngayKetThuc && f.ngayBatDau > f.ngayKetThuc) return 'Start date must be before end date.';
    return null;
  }
}