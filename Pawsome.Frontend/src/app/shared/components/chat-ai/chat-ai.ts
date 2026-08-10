import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-chat-ai',
  templateUrl: './chat-ai.html',
  styleUrl: './chat-ai.scss'
})
export class ChatAi {
  readonly dangMo = signal(false);

  toggle(): void {
    this.dangMo.update(v => !v);
  }
}
