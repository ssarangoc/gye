import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

/**
 * Rutas con lazy loading: cada feature se descarga solo cuando el usuario
 * navega a ella, en vez de incluirse en el bundle inicial.
 */
export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'products',
    canActivate: [authGuard],
    loadComponent: () => import('./features/products/products.component').then((m) => m.ProductsComponent)
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
