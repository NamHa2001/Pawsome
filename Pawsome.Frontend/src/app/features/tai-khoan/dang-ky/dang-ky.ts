import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-dang-ky',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './dang-ky.html',
  styleUrl: './dang-ky.css'
})
export class DangKyComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  dangGui = false;
  loiChung: string | null = null;

  form = this.fb.nonNullable.group({
    hoTen: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    soDienThoai: [''],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dangGui = true;
    this.loiChung = null;

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.dangGui = false;
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.dangGui = false;
        this.loiChung = err?.error?.message ?? 'Đăng ký thất bại. Vui lòng thử lại.';
      }
    });
  }
}