import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Address, AddressFormValue, AddressService } from '../address.service';

@Component({
  selector: 'app-dia-chi',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Header, Footer, ChatAi],
  templateUrl: './dia-chi.html',
  styleUrl: './dia-chi.scss'
})
export class DiaChiComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly addressService = inject(AddressService);

  danhSach: Address[] = [];
  dangTai = true;
  dangGui = false;
  loiChung: string | null = null;

  hienForm = false;
  dangSuaId: number | null = null;

  form = this.fb.nonNullable.group({
    nguoiNhan: ['', [Validators.required]],
    soDienThoai: ['', [Validators.required]],
    diaChiChiTiet: ['', [Validators.required]],
    phuongXa: [''],
    quanHuyen: [''],
    tinhThanh: ['', [Validators.required]],
    laMacDinh: [false]
  });

  ngOnInit(): void {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai = true;
    this.addressService.getAll().subscribe({
      next: (res) => {
        this.dangTai = false;
        this.danhSach = res.data ?? [];
      },
      error: () => {
        this.dangTai = false;
        this.loiChung = 'Could not load your addresses. Please try again.';
      }
    });
  }

  moFormThemMoi(): void {
    this.dangSuaId = null;
    this.form.reset({ laMacDinh: false });
    this.hienForm = true;
    this.loiChung = null;
  }

  moFormSua(address: Address): void {
    this.dangSuaId = address.addressId;
    this.form.setValue({
      nguoiNhan: address.nguoiNhan,
      soDienThoai: address.soDienThoai,
      diaChiChiTiet: address.diaChiChiTiet,
      phuongXa: address.phuongXa ?? '',
      quanHuyen: address.quanHuyen ?? '',
      tinhThanh: address.tinhThanh,
      laMacDinh: address.laMacDinh
    });
    this.hienForm = true;
    this.loiChung = null;
  }

  dongForm(): void {
    this.hienForm = false;
    this.dangSuaId = null;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    const dto: AddressFormValue = this.form.getRawValue();
    const request$ = this.dangSuaId
      ? this.addressService.update(this.dangSuaId, dto)
      : this.addressService.create(dto);

    request$.subscribe({
      next: () => {
        this.dangGui = false;
        this.dongForm();
        this.taiDanhSach();
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Something went wrong. Please try again.';
      }
    });
  }

  xoa(address: Address): void {
    const xacNhan = confirm(`Delete this address: "${address.diaChiChiTiet}"?`);
    if (!xacNhan) return;

    this.addressService.delete(address.addressId).subscribe({
      next: () => this.taiDanhSach(),
      error: (err) => {
        this.loiChung = err?.error?.message ?? 'Could not delete this address.';
      }
    });
  }
}