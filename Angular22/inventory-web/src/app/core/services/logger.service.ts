import { Injectable } from '@angular/core';

/**
 * Logging estructurado del front-end (equivalente conceptual a Serilog en el back).
 * Centraliza el formato de los logs para poder redirigirlos a un servicio externo
 * (Application Insights, Sentry, etc.) sin tocar el resto de la app.
 */
@Injectable({ providedIn: 'root' })
export class LoggerService {
  info(message: string, context?: unknown): void {
    this.write('INFO', message, context);
  }

  warn(message: string, context?: unknown): void {
    this.write('WARN', message, context);
  }

  error(message: string, context?: unknown): void {
    this.write('ERROR', message, context);
  }

  private write(level: 'INFO' | 'WARN' | 'ERROR', message: string, context?: unknown): void {
    const entry = `[${new Date().toISOString()}] [${level}] ${message}`;
    const logFn = level === 'ERROR' ? console.error : level === 'WARN' ? console.warn : console.info;
    if (context !== undefined) {
      logFn(entry, context);
    } else {
      logFn(entry);
    }
  }
}
