import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../auth.service';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

declare const google: any;

@Component({
  selector: 'app-dang-nhap',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './dang-nhap.html',
  styleUrl: './dang-nhap.scss'
})
export class DangNhapComponent implements AfterViewInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  @ViewChild('googleBtn') googleBtn!: ElementRef<HTMLDivElement>;

  dangGui = false;
  loiChung: string | null = null;

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  ngAfterViewInit(): void {
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: { credential: string }) => this.xuLyGoogleLogin(response.credential)
    });

    google.accounts.id.renderButton(this.googleBtn.nativeElement, {
      theme: 'outline',
      size: 'large',
      width: 340,
      text: 'signin_with',
      locale: 'vi'
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => {
        this.dangGui = false;
        this.dieuHuongSauDangNhap();
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Sign in failed. Please try again.';
      }
    });
  }

  private xuLyGoogleLogin(idToken: string): void {
    this.loiChung = null;

    this.authService.googleLogin({ idToken }).subscribe({
      next: () => this.dieuHuongSauDangNhap(),
      error: (err) => {
        this.loiChung =
          err?.error?.message ??
          'Google sign-in failed. Please try again.';
      }
    });
  }

  // Admin vào thẳng dashboard quản trị; Moderator chỉ được vào trang kiểm duyệt đánh giá
  // (route 'quan-tri' mặc định redirect sang 'dashboard', mà dashboard chỉ cho phép Admin -
  // nếu điều Moderator vào '/quan-tri' sẽ bị roleGuard bật ngược về trang chủ). Customer/
  // Support về trang chủ như cũ.
  private dieuHuongSauDangNhap(): void {
    if (this.tokenService.hasRole('Admin')) {
      this.router.navigate(['/quan-tri']);
    } else if (this.tokenService.hasRole('Moderator')) {
      this.router.navigate(['/quan-tri/kiem-duyet-danh-gia']);
    } else {
      this.router.navigate(['/']);
    }
  }
}