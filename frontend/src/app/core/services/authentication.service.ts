import { inject, Injectable, signal } from '@angular/core';
import { Authentication } from '../../shared/models/Authentication.model';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

import { HttpService } from './http.service';
import { LoaderService } from './loader.service';
import { GlobalDataService } from './global-data.service';
import { NotificationsService } from './notifications.service';

import { Utils } from '../../shared/Utils/Utils';
import { ILogin } from '../../shared/interfaces/ILogin.interface';
import { IToken } from '../../shared/interfaces/IToken.interface';
import { IAuthGet } from '../../shared/api-contracts/IAuthGet.interface';
import { Constants } from '../../shared/Utils/Constants';
import { ApiResponse } from '../../shared/models/ApiResponse.model';
import { ErrorMessage } from '../../shared/Utils/ErrorMessage';
import { AppNotification } from '../../shared/models/AppNotification.model';
import { IAuthenticationGet } from '../../shared/api-contracts/IAuthenticationGet.interface';
import { User } from '../../shared/models/User.model';
import { SuccessMessage } from '../../shared/Utils/SuccessMessage';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private _router = inject(Router);
  private _httpService = inject(HttpService);
  private _notificationsService = inject(NotificationsService);
  private _loaderService = inject(LoaderService);
  private _globalDataService = inject(GlobalDataService);
  private _auth = signal<Authentication>(Authentication.getUnauthenticated());

  public auth = this._auth.asReadonly();

  public async login(login: ILogin): Promise<void> {
    try {
      this._loaderService.setLoading();
      localStorage.removeItem('token');

      const response = await this._httpService.makeRequestAsync<IAuthGet>(
        'POST',
        Constants.LOGIN_END_POINT,
        login
      );

      if (Utils.isRequestFailure(response)) return;
      const token: string = response?.data.token!;

      this.setToken(token);

      this.loginHelper(token);
    } catch (error: any) {
      const message =
        error.status === 500
          ? ErrorMessage.fetchError
          : error.status === 403
          ? ErrorMessage.inactive
          : ErrorMessage.unauthorized;

      console.log('File: authentication.service.ts', 'Line: 54', message);
      this._notificationsService.openNotification(new AppNotification(message));
    } finally {
      this._loaderService.setLoading();
    }
  }

  public async autoLogin(): Promise<void> {
    try {
      this._loaderService.setLoading();

      const response = await this.refreshToken();
      if (Utils.isRequestFailure(response)) return;

      const token: string = response?.data.token!;

      this.setToken(token);

      this.loginHelper(token);
    } catch (error: any) {
      const notification = new AppNotification(
        ErrorMessage.unauthorized,
        'error',
        true
      );
      this._notificationsService.openNotification(notification);
    } finally {
      this._loaderService.setLoading();
    }
  }

  private async loginHelper(token: string): Promise<void> {
    const user = await this.setUser(token);

    if (user?.partnerId === null && user?.clientId === null)
      this._globalDataService.cacheData();
    else this._globalDataService.setYearOptions();

    this._router.navigate(['/protocolo']);
  }

  private async setUser(token: string): Promise<User | null> {
    const decodedToken = this.getDecodedToken();
    if (!decodedToken) return null;
    const { userId } = decodedToken;
    const authentication =
      await this._httpService.makeRequestAsync<IAuthenticationGet>(
        'GET',
        `${Constants.AUTHENTICATION_END_POINT}/${userId}`
      );
    const updateAuth = Authentication.fromInterface(
      authentication?.data,
      token,
      userId
    );

    this._auth.set(updateAuth);

    return updateAuth.user;
  }

  public logout(): void {
    this._auth.set(Authentication.getUnauthenticated());
    localStorage.removeItem('token');
    this._router.navigate(['/login']);
  }

  public async isAuthenticated(): Promise<boolean> {
    try {
      const token = this.getToken();
      const data = { token };
      const response = await this._httpService.makeRequestAsync<IAuthGet>(
        'POST',
        Constants.TOKEN_VALIDATOR_END_POINT,
        data
      );
      if (!response?.data) return false;
      await this.setUser(token!);
      return true;
    } catch (error) {
      return false;
    }
  }

  public setToken(token: string): void {
    localStorage.setItem('token', token);
  }

  public getToken(): string | null {
    const tokenStr = localStorage.getItem('token');
    if (!tokenStr) return null;
    return tokenStr;
  }

  public isTokenExpired(): boolean {
    const decodedToken = this.getDecodedToken();
    if (!decodedToken) return false;
    try {
      const now = Math.floor(Date.now() / 1000);
      return decodedToken.exp < now;
    } catch (error) {
      return false;
    }
  }

  public async refreshToken(): Promise<ApiResponse<IAuthGet> | null> {
    const token = this.getToken();
    const data = { refreshToken: token };
    const response = await this._httpService.makeRequestAsync<IAuthGet>(
      'POST',
      Constants.REFRESH_END_POINT,
      data
    );
    return response;
  }

  private getDecodedToken(): IToken | null {
    const token = this.getToken();
    if (!token) return null;

    return jwtDecode<IToken>(token);
  }
}
