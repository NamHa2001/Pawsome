import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../auth.service';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';

type BuocForm = 'nhap-email' | 'nhap-otp';

@Component({
  selector: 'app-quen-mat-khau',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './quen-mat-khau.html',
  styleUrl: './quen-mat-khau.scss'
})
export class QuenMatKhauComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  buoc: BuocForm = 'nhap-email';
  dangGui = false;
  loiChung: string | null = null;

  private resetToken: string | null = null;
  emailDaNhap = '';

  emailForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]]
  });

  otpForm = this.fb.nonNullable.group({
    otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]]
  });

  submitEmail(): void {
    if (this.emailForm.invalid) {
      this.emailForm.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    const { email } = this.emailForm.getRawValue();

    this.authService.forgotPassword({ email }).subscribe({
      next: (res) => {
        this.dangGui = false;
        this.resetToken = res.data?.resetToken ?? null;
        this.emailDaNhap = email;
        this.buoc = 'nhap-otp';
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Something went wrong. Please try again.';
      }
    });
  }

  submitOtp(): void {
    if (this.otpForm.invalid || !this.resetToken) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    const { otp, newPassword } = this.otpForm.getRawValue();

    this.authService.resetPassword({ token: this.resetToken, otp, newPassword }).subscribe({
      next: () => {
        this.dangGui = false;
        this.router.navigate(['/tai-khoan/dang-nhap']);
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Invalid or expired code. Please try again.';
      }
    });
  }

  quayLaiNhapEmail(): void {
    this.buoc = 'nhap-email';
    this.resetToken = null;
    this.loiChung = null;
    this.otpForm.reset();
  }
}