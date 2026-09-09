import { DecimalPipe } from '@angular/common';
import { Component, ElementRef, ViewChild, effect, inject, signal } from '@angular/core';
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
    // setTimeout(0) để đợi Angular render xong *ngFor mới rồi mới đọc scrollHeight, đọc ngay trong
    // effect sẽ lấy phải chiều cao CŨ (trước khi DOM cập nhật tin nhắn vừa thêm).
    effect(() => {
      this.lichSu();
      setTimeout(() => this.cuonXuongCuoi());
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
