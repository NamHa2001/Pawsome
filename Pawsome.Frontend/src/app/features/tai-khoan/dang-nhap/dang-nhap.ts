import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../auth.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

declare const google: any;

@Component({
  selector: 'app-dang-nhap',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink,Header,Footer,ChatAi],
  templateUrl: './dang-nhap.html',
  styleUrl: './dang-nhap.scss'
})
export class DangNhapComponent implements AfterViewInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
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
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Đăng nhập thất bại. Vui lòng thử lại.';
      }
    });
  }

  private xuLyGoogleLogin(idToken: string): void {
    this.loiChung = null;
    this.authService.googleLogin({ idToken }).subscribe({
      next: () => this.router.navigate(['/']),
      error: (err) => {
        this.loiChung = err?.error?.message ?? 'Đăng nhập Google thất bại. Vui lòng thử lại.';
      }
    });
  }
}