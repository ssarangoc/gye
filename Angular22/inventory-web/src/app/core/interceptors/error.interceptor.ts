import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { LoggerService } from '../services/logger.service';
import { NotificationService } from '../services/notification.service';

/**
 * Manejo centralizado de errores HTTP: registra el error, muestra un mensaje
 * personalizado al usuario y fuerza el logout cuando el token ya no es válido.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const logger = inject(LoggerService);
  const notifications = inject(NotificationService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      logger.error(`Fallo en ${req.method} ${req.url}`, { status: error.status, body: error.error });

      const message = extractMessage(error);

      if (error.status === 401) {
        notifications.error('Tu sesión expiró o no es válida. Vuelve a iniciar sesión.');
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        router.navigate(['/login']);
      } else if (error.status === 0) {
        notifications.error('No se pudo conectar con el servidor. Verifica tu conexión.');
      } else if (!req.url.endsWith('/api/auth/login')) {
        // El login ya muestra su propio mensaje de error en el formulario.
        notifications.error(message);
      }

      return throwError(() => error);
    })
  );
};

function extractMessage(error: HttpErrorResponse): string {
  if (typeof error.error === 'string' && error.error.trim().length > 0) {
    return error.error;
  }
  if (error.error?.message) {
    return error.error.message;
  }
  return 'Ocurrió un error inesperado. Intenta nuevamente.';
}
