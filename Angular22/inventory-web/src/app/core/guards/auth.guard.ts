import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard funcional (estilo moderno de Angular) que protege rutas privadas.
 * Redirige a /login cuando no hay un token válido en AuthService.
 */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
