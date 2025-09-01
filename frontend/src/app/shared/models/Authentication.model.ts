import { IAuthenticationGet } from '../api-contracts/IAuthenticationGet.interface';
import { IAuthentication } from '../interfaces/IAuthentication.interface';
import { Permission } from './Permission.model';
import { User } from './User.model';

export class Authentication implements IAuthentication {
  constructor(
    private _user: User | null,
    private _permission: Permission | null,
    private _token: string | null
  ) {}

  public static getUnauthenticated(): Authentication {
    return new Authentication(null, null, null);
  }

  get user(): User | null {
    return this._user;
  }

  get token(): string | null {
    return this._token;
  }

  get permission(): Permission | null {
    return this._permission;
  }

  get isLogin(): boolean {
    return (
      this._user !== null && this._token !== null 
    );
  }

  public static fromInterface(
    authentication: IAuthenticationGet | undefined,
    token: string,
    userId: string
  ): Authentication {
    if (!authentication) return this.getUnauthenticated();

    const user = User.fromInterface(authentication, userId);
    const permission = Permission.fromInterface(authentication);

    return new Authentication(user, permission, token);
  }
}
