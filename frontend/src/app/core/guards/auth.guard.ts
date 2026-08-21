import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

export const authGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);

  if (userService.isLoggedIn()) {
    return true;
  }

  // Avoid setting returnUrl to /login to prevent infinite returnUrl loops
  const returnUrl = (state.url === '/login' || !state.url) ? '/' : state.url;

  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl }
  });
};
