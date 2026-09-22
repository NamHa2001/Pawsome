import { AfterViewInit, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { SubscribeService } from '../services/subscribe.service';

declare const grecaptcha: any;

@Component({
  selector: 'app-welcome-bonus',
  imports: [RouterLink, FormsModule],
  templateUrl: './welcome-bonus.html',
  styleUrl: './welcome-bonus.scss'
})
export class WelcomeBonus implements AfterViewInit {
  private readonly subscribeService = inject(SubscribeService);

  @ViewChild('recaptchaBox') recaptchaBox!: ElementRef<HTMLDivElement>;
  @ViewChild('emailInput') emailInput!: ElementRef<HTMLInputElement>;

  email = '';
  readonly recaptchaToken = signal<string | null>(null);
  readonly dangGui = signal(false);
  readonly thongBao = signal<string | null>(null);
  readonly loi = signal<string | null>(null);

  private recaptchaWidgetId: number | null = null;

  ngAfterViewInit(): void {
    this.renderRecaptchaKhiSanSang();
  }

  // Script api.js load async - đôi khi Angular chạy ngAfterViewInit trước khi Google gán xong
  // hàm grecaptcha.render (biến grecaptcha tồn tại nhưng .render chưa phải function), gây lỗi
  // "grecaptcha.render is not a function". Tự chờ tới khi thật sự sẵn sàng thay vì gọi thẳng.
  private renderRecaptchaKhiSanSang(soLanThu = 0): void {
    if (typeof grecaptcha !== 'undefined' && typeof grecaptcha.render === 'function') {
      this.recaptchaWidgetId = grecaptcha.render(this.recaptchaBox.nativeElement, {
        sitekey: environment.recaptchaSiteKey,
        callback: (token: string) => this.recaptchaToken.set(token),
        'expired-callback': () => this.recaptchaToken.set(null)
      });
      return;
    }

    if (soLanThu > 50) return;
    setTimeout(() => this.renderRecaptchaKhiSanSang(soLanThu + 1), 200);
  }

  // Dùng cho [disabled] trên nút Subscribe - vô hiệu nút ngay khi thiếu email/chưa tick reCAPTCHA
  // thay vì để bấm được rồi mới báo lỗi (subscribe() bên dưới vẫn giữ nguyên check này làm lớp bảo
  // vệ cuối, phòng trường hợp autofill khiến template chưa kịp cập nhật lại disabled).
  thieuDieuKien(): boolean {
    return !this.email.trim() || !this.recaptchaToken();
  }

  subscribe(): void {
    this.loi.set(null);
    this.thongBao.set(null);

    // Đọc thẳng giá trị thật từ DOM thay vì tin vào this.email - trình duyệt tự động điền
    // (autofill) đôi khi không báo cho ngModel biết giá trị đã thay đổi.
    const email = this.emailInput.nativeElement.value.trim();
    if (!email) {
      this.loi.set('Please enter your email.');
      return;
    }

    const token = this.recaptchaToken();
    if (!token) {
      this.loi.set('Please verify that you are not a robot.');
      return;
    }

    this.dangGui.set(true);
    this.subscribeService.subscribe({ email, recaptchaToken: token }).subscribe({
      next: res => {
        this.dangGui.set(false);
        const maCode = res.data?.maCode;
        this.thongBao.set(maCode ? `Subscribed! Your discount code: ${maCode}` : res.message ?? 'Subscribed successfully!');
        this.email = '';
        this.recaptchaToken.set(null);
        if (this.recaptchaWidgetId !== null) {
          grecaptcha.reset(this.recaptchaWidgetId);
        }
      },
      error: err => {
        this.dangGui.set(false);
        this.loi.set(err?.error?.message ?? 'Subscription failed. Please try again.');
        if (this.recaptchaWidgetId !== null) {
          grecaptcha.reset(this.recaptchaWidgetId);
        }
        this.recaptchaToken.set(null);
      }
    });
  }
}
