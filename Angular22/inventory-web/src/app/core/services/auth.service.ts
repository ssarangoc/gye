import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponse, CurrentUser, LoginRequest, RegisterRequest, RegisterResponse } from '../models/auth.model';
import { LoggerService } from './logger.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5270/api/auth';

  /** Señal reactiva con el estado de autenticación, útil para bindings en templates. */
  readonly isLoggedIn = signal<boolean>(this.hasToken());

  constructor(private http: HttpClient, private logger: LoggerService) {}

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('role', response.role);
        this.isLoggedIn.set(true);
        this.logger.info(`Login exitoso para ${response.username}`);
      })
    );
  }

  register(payload: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, payload);
  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.http.get<CurrentUser>(`${this.apiUrl}/me`);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    this.isLoggedIn.set(false);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }
}
