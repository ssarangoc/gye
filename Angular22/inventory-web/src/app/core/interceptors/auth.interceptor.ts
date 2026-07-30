import { HttpInterceptorFn } from '@angular/common/http';

/**
 * Adjunta automáticamente el token JWT a todas las peticiones salientes,
 * evitando repetir la lógica de headers en cada servicio (DRY).
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  if (!token) {
    return next(req);
  }

  const authorizedReq = req.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  });

  return next(authorizedReq);
};
