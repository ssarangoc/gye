import { Injectable, signal } from '@angular/core';

export type NotificationType = 'success' | 'error' | 'info';

export interface Notification {
  id: number;
  type: NotificationType;
  message: string;
}

/**
 * Muestra mensajes personalizados al usuario (toasts) sin acoplar
 * cada componente a la lógica de presentación de errores/éxitos.
 */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private nextId = 0;
  readonly notifications = signal<Notification[]>([]);

  success(message: string): void {
    this.push('success', message);
  }

  error(message: string): void {
    this.push('error', message);
  }

  info(message: string): void {
    this.push('info', message);
  }

  dismiss(id: number): void {
    this.notifications.update((items) => items.filter((item) => item.id !== id));
  }

  private push(type: NotificationType, message: string): void {
    const id = this.nextId++;
    this.notifications.update((items) => [...items, { id, type, message }]);
    setTimeout(() => this.dismiss(id), 5000);
  }
}
