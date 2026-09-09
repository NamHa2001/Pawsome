import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { catchError, finalize, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ChatRequest, ChatResponse, TinNhanChat } from './chat-ai.model';

// providedIn: 'root' (singleton) để lịch sử hội thoại còn giữ được khi người dùng điều hướng
// sang trang khác trong SPA - <app-chat-ai> được đặt lại trong template của MỖI trang nên bản
// thân component sẽ bị hủy/tạo lại mỗi lần chuyển route, nếu giữ state trong component sẽ mất
// hội thoại ngay khi chuyển trang. Không lưu xuống CSDL/localStorage (theo đúng lựa chọn "không
// lưu lịch sử" đã chốt) - chỉ sống trong bộ nhớ JS của tab hiện tại, refresh trang là mất.
@Injectable({ providedIn: 'root' })
export class ChatAiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/ai`;

  readonly lichSu = signal<TinNhanChat[]>([
    { vai: 'bot', noiDung: 'Xin chào! Mình là PawSome AI Assistant. Kể mình nghe về thú cưng hoặc triệu chứng bé đang gặp, mình sẽ gợi ý sản phẩm phù hợp trên Pawsome nhé! 🐾' }
  ]);
  readonly dangGui = signal(false);

  guiTinNhan(noiDung: string): void {
    const noiDungSach = noiDung.trim();
    if (!noiDungSach || this.dangGui()) return;

    // Snapshot lịch sử TRƯỚC khi thêm câu hỏi mới - đúng dữ liệu backend cần trong
    // ChatRequestDto.LichSu (các lượt hội thoại trước đó, chưa tính câu hỏi lần này).
    const lichSuGuiLen = this.lichSu().map(t => ({ vai: t.vai, noiDung: t.noiDung }));

    this.lichSu.update(ds => [...ds, { vai: 'nguoi_dung', noiDung: noiDungSach }]);
    this.dangGui.set(true);

    const request: ChatRequest = { tinNhanMoi: noiDungSach, lichSu: lichSuGuiLen };

    this.http.post<ApiResponse<ChatResponse>>(`${this.baseUrl}/chat`, request)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          const thongBaoLoi = err.error?.message ?? 'Xin lỗi, hiện tại mình không phản hồi được. Bạn thử lại sau nhé.';
          return of<ApiResponse<ChatResponse>>({ success: false, data: null, message: thongBaoLoi });
        }),
        finalize(() => this.dangGui.set(false))
      )
      .subscribe(res => {
        const traLoi = res.data?.traLoi ?? res.message ?? 'Xin lỗi, mình không phản hồi được.';
        this.lichSu.update(ds => [...ds, { vai: 'bot', noiDung: traLoi, sanPham: res.data?.sanPham ?? [] }]);
      });
  }
}
