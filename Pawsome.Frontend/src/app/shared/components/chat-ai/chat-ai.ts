import { DecimalPipe } from '@angular/common';
import { Component, ElementRef, ViewChild, afterRenderEffect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChatAiService } from './chat-ai.service';

@Component({
  selector: 'app-chat-ai',
  imports: [FormsModule, RouterLink, DecimalPipe],
  templateUrl: './chat-ai.html',
  styleUrl: './chat-ai.scss'
})
export class ChatAi {
  private readonly chatAiService = inject(ChatAiService);

  readonly dangMo = signal(false);
  readonly noiDungGo = signal('');

  readonly lichSu = this.chatAiService.lichSu;
  readonly dangGui = this.chatAiService.dangGui;

  @ViewChild('khungNoiDungChat') khungNoiDungChat?: ElementRef<HTMLDivElement>;

  constructor() {
    // Tự cuộn xuống tin nhắn mới nhất mỗi khi lịch sử thay đổi (câu hỏi mới hoặc AI vừa trả lời).
    // afterRenderEffect (không phải effect thường) vì cần đọc scrollHeight SAU khi Angular render
    // xong *ngFor mới - effect thường chạy trước khi DOM kịp cập nhật tin nhắn vừa thêm.
    afterRenderEffect(() => {
      this.lichSu();
      this.cuonXuongCuoi();
    });
  }

  toggle(): void {
    this.dangMo.update(v => !v);
    if (this.dangMo()) this.cuonXuongCuoi();
  }

  gui(): void {
    const noiDung = this.noiDungGo();
    if (!noiDung.trim() || this.dangGui()) return;

    this.chatAiService.guiTinNhan(noiDung);
    this.noiDungGo.set('');
  }

  private cuonXuongCuoi(): void {
    const el = this.khungNoiDungChat?.nativeElement;
    if (el) el.scrollTop = el.scrollHeight;
  }
}
