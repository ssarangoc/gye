import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="login-shell">
      <form class="card" [formGroup]="form" (ngSubmit)="login()">
        <h2>Iniciar sesión</h2>
        <label>Usuario</label>
        <input formControlName="username" />
        <label>Contraseña</label>
        <input type="password" formControlName="password" />
        <button type="submit" [disabled]="form.invalid">Entrar</button>
        <p class="error">{{ error() }}</p>
      </form>
    </div>
  `,
  styles: [
    `.login-shell{min-height:100vh;display:grid;place-items:center;background:#f4f6fb;padding:24px;}`,
    `.card{background:white;padding:24px;border-radius:16px;box-shadow:0 12px 30px rgba(0,0,0,.12);display:flex;flex-direction:column;gap:12px;width:100%;max-width:360px;}`,
    `input{padding:10px;border:1px solid #cbd5e1;border-radius:8px;width:100%;font-size:16px;}`,
    `button{padding:11px;border:none;border-radius:8px;background:#2563eb;color:white;cursor:pointer;}`,
    `button:disabled{background:#94a3b8;cursor:not-allowed;}`,
    `.error{color:#dc2626;min-height:20px;}`,
    `@media (max-width: 480px){.login-shell{padding:12px;}.card{padding:18px;border-radius:12px;}}`
  ]
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);

  readonly error = signal('');

  readonly form = this.fb.group({
    username: ['admin', Validators.required],
    password: ['admin', Validators.required]
  });

  constructor(private auth: AuthService, private router: Router) {}

  login(): void {
    this.error.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.getRawValue();
    this.auth.login({ username: username ?? '', password: password ?? '' }).subscribe({
      next: () => this.router.navigate(['/products']),
      error: () => this.error.set('Credenciales inválidas')
    });
  }
}
