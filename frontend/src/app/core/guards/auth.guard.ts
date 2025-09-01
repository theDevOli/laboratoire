import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthenticationService } from '../services/authentication.service';

export const authGuard: CanActivateFn = async (route, state) => {
  const authenticationService = inject(AuthenticationService);

  if (authenticationService.auth().isLogin) return true;

  const token = authenticationService.getToken();
  if (token && (await authenticationService.isAuthenticated())) return true;

  localStorage.removeItem('token');
  
  const router = inject(Router);

  return router.createUrlTree(['/login']);
};
