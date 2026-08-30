import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { AuthService } from '../auth.service';
import { UserProfile, UserService } from '../user.service';
import { Address, AddressService } from '../address.service';
import { DANH_SACH_GOI_PAWVIP } from '../../gio-hang/pawvip/models/pawvip-goi.model';
import { TaiKhoanSidebarComponent } from '../tai-khoan-sidebar/tai-khoan-sidebar';


function matKhauKhopValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const moi = group.get('matKhauMoi')?.value;
    const xacNhan = group.get('xacNhanMatKhauMoi')?.value;
    return moi === xacNhan ? null : { khongKhop: true };
  };
}

@Component({
  selector: 'app-ho-so',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Header, Footer, ChatAi, TaiKhoanSidebarComponent],
  templateUrl: './ho-so.html',
  styleUrl: './ho-so.scss'
})
export class HoSoComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly addressService = inject(AddressService);
  private readonly authService = inject(AuthService);

  hoSo = signal<UserProfile | null>(null);
  diaChiList = signal<Address[]>([]);

  dangTai = true;
  dangSuaHoSo = false;
  dangGui = false;
  loiChung: string | null = null;
  thongBaoThanhCong: string | null = null;

  dangSuaMatKhau = false;
  dangGuiMatKhau = false;
  loiMatKhau: string | null = null;
  thongBaoMatKhau: string | null = null;

  form = this.fb.nonNullable.group({
    hoTen: ['', [Validators.required]],
    soDienThoai: ['']
  });

  matKhauForm = this.fb.nonNullable.group({
    matKhauCu: ['', [Validators.required]],
    matKhauMoi: ['', [Validators.required, Validators.minLength(6)]],
    xacNhanMatKhauMoi: ['', [Validators.required]]
  }, { validators: matKhauKhopValidator() });

  ngOnInit(): void {
    this.taiHoSo();
    this.addressService.getAll().subscribe({
      next: (res) => this.diaChiList.set(res.data ?? [])
    });
  }

  // Trước đây khối "PawVip Membership" ở trang này chỉ là hàng trang trí tĩnh (nút JOIN NOW
  // không có (click), không link đi đâu) - giờ đọc đúng trạng thái PawVip thật của khách, khớp
  // đúng danh sách gói đang hiển thị ở trang PawVip (gio-hang/pawvip).
  readonly goiPawVipHienTai = computed(() => {
    const tier = this.hoSo()?.pawVipTier;
    return tier ? DANH_SACH_GOI_PAWVIP.find(g => g.id === tier) : undefined;
  });

  taiHoSo(): void {
    this.dangTai = true;
    this.userService.getProfile().subscribe({
      next: (res) => {
        this.dangTai = false;
        if (res.data) {
          this.hoSo.set(res.data);
          this.form.setValue({
            hoTen: res.data.hoTen,
            soDienThoai: res.data.soDienThoai ?? ''
          });
        }
      },
      error: () => {
        this.dangTai = false;
        this.loiChung = 'Could not load your profile. Please try again.';
      }
    });
  }

  moSuaHoSo(): void {
    this.dangSuaHoSo = true;
    this.dangSuaMatKhau = false;
    this.thongBaoThanhCong = null;
  }

  huySuaHoSo(): void {
    this.dangSuaHoSo = false;
    const hs = this.hoSo();
    if (hs) this.form.setValue({ hoTen: hs.hoTen, soDienThoai: hs.soDienThoai ?? '' });
  }

  luuHoSo(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    this.userService.updateProfile(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.dangGui = false;
        this.dangSuaHoSo = false;
        this.thongBaoThanhCong = 'Profile updated successfully.';
        if (res.data) this.hoSo.set(res.data);
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Something went wrong. Please try again.';
      }
    });
  }

  moSuaMatKhau(): void {
    this.dangSuaMatKhau = true;
    this.dangSuaHoSo = false;
    this.thongBaoMatKhau = null;
    this.loiMatKhau = null;
  }

  huySuaMatKhau(): void {
    this.dangSuaMatKhau = false;
    this.matKhauForm.reset({ matKhauCu: '', matKhauMoi: '', xacNhanMatKhauMoi: '' });
  }

  luuMatKhau(): void {
    if (this.matKhauForm.invalid) {
      this.matKhauForm.markAllAsTouched();
      if (this.matKhauForm.errors?.['khongKhop']) {
        this.loiMatKhau = 'Mật khẩu xác nhận không khớp với mật khẩu mới.';
      }
      return;
    }

    this.dangGuiMatKhau = true;
    this.loiMatKhau = null;

    const { matKhauCu, matKhauMoi } = this.matKhauForm.getRawValue();

    this.authService.changePassword({ matKhauCu, matKhauMoi }).subscribe({
      next: () => {
        this.dangGuiMatKhau = false;
        this.dangSuaMatKhau = false;
        this.thongBaoMatKhau = 'Đổi mật khẩu thành công.';
        this.matKhauForm.reset({ matKhauCu: '', matKhauMoi: '', xacNhanMatKhauMoi: '' });
      },
      error: (err) => {
        this.dangGuiMatKhau = false;
        this.loiMatKhau = err?.error?.message ?? 'Đổi mật khẩu thất bại. Vui lòng thử lại.';
      }
    });
  }
}