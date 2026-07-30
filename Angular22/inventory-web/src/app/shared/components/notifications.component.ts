import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';

/**
 * Componente global de notificaciones (toasts) montado una sola vez en app.ts.
 */
@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      <div
        class="toast"
        [class.success]="n.type === 'success'"
        [class.error]="n.type === 'error'"
        [class.info]="n.type === 'info'"
        *ngFor="let n of notificationService.notifications()"
      >
        <span>{{ n.message }}</span>
        <button type="button" (click)="notificationService.dismiss(n.id)">×</button>
      </div>
    </div>
  `,
  styles: [
    `.toast-container{position:fixed;top:16px;right:16px;display:flex;flex-direction:column;gap:8px;z-index:1000;max-width:min(90vw,360px);}`,
    `.toast{display:flex;justify-content:space-between;align-items:center;gap:12px;padding:12px 14px;border-radius:10px;color:white;box-shadow:0 8px 20px rgba(0,0,0,.15);font-size:14px;}`,
    `.toast.error{background:#dc2626;}`,
    `.toast.success{background:#16a34a;}`,
    `.toast.info{background:#2563eb;}`,
    `.toast button{background:transparent;border:none;color:white;font-size:16px;cursor:pointer;line-height:1;padding:0;}`
  ]
})
export class NotificationsComponent {
  constructor(public notificationService: NotificationService) {}
}
