import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-dang-nhap',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './dang-nhap.html',
  styleUrl: './dang-nhap.css'
})
export class DangNhapComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  dangGui = false;
  loiChung: string | null = null;

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

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
}