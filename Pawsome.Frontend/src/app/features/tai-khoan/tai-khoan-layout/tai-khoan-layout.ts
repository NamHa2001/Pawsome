import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { TaiKhoanSidebarComponent } from '../tai-khoan-sidebar/tai-khoan-sidebar';

// Layout dùng chung cho khu tài khoản (Dashboard, Manage Account, Shipping Address, Order
// History, PawPoints, Auto Orders...) - trước đây mỗi trang tự nhúng lại
// <app-header>/<app-tai-khoan-sidebar>/<app-footer> nên trang nào quên nhúng (Order History,
// PawPoints, Auto Orders, Shipping Address) sẽ mất hẳn sidebar và bật hẳn sang view rời rạc dù
// vẫn nằm trong khu tài khoản. Gộp về 1 route cha có layout này + <router-outlet> để mọi trang
// con tự động có cùng khung sidebar, không phải tự lo.
@Component({
  selector: 'app-tai-khoan-layout',
  standalone: true,
  imports: [RouterOutlet, Header, Footer, ChatAi, TaiKhoanSidebarComponent],
  templateUrl: './tai-khoan-layout.html',
  styleUrl: './tai-khoan-layout.scss'
})
export class TaiKhoanLayoutComponent {}
