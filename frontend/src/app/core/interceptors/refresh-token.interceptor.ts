import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { inject } from '@angular/core';

import { AuthenticationService } from '../services/authentication.service';

import { IAuthGet } from '../../shared/api-contracts/IAuthGet.interface';
import { AppNotification } from '../../shared/models/AppNotification.model';
import { ErrorMessage } from '../../shared/Utils/ErrorMessage';
import { NotificationsService } from '../services/notifications.service';

export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authenticationService = inject(AuthenticationService);
  const notificationsService = inject(NotificationsService);

  if (authenticationService.isTokenExpired()) {
    return from(authenticationService.refreshToken()).pipe(
      switchMap((res) => {
        if (res !== null) {
          const token: IAuthGet = res.data;
          authenticationService.setToken(token.token);

          const clonedRequest = req.clone({
            setHeaders: {
              Authorization: `Bearer ${token.token}`,
            },
          });

          return next(clonedRequest);
        } else return next(req);
      }),
      catchError((error) => {
        console.error('Token refresh failed', error);
        authenticationService.logout();
        const notification = new AppNotification(
          ErrorMessage.refreshToken,
          'info',
          true
        );
        notificationsService.openNotification(notification);
        return throwError(() => error);
      })
    );
  }
  return next(req);
};
