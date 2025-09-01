import { IAuthenticationGet } from '../api-contracts/IAuthenticationGet.interface';

export class User {
  constructor(
    private _userId: string,
    private _partnerId: string | null,
    private _clientId: string | null,
    private _username: string,
    private _name: string,
    private _isActive: boolean
  ) {}

  public get userId(): string {
    return this._userId;
  }

  public get partnerId(): string | null {
    return this._partnerId;
  }
  public get clientId(): string | null {
    return this._clientId;
  }

  public get username(): string {
    return this._username;
  }

  public get name(): string {
    return this._name;
  }

  public get isActive(): boolean {
    return this._isActive;
  }

  public set userId(userId: string) {
    this.userId = userId;
  }

  public set partnerId(partnerId: string | null) {
    this._partnerId = partnerId;
  }
  public set clientId(clientId: string | null) {
    this._clientId = clientId;
  }

  public set username(username: string) {
    this._username = username;
  }

  public set name(name: string) {
    this._name = name;
  }

  public set isActive(isActive: boolean) {
    this._isActive = isActive;
  }

  public static fromInterface(
    authentication: IAuthenticationGet,
    userId: string
  ): User {
    return new User(
      userId,
      authentication.partnerId,
      authentication.clientId,
      authentication.username,
      authentication.name,
      authentication.isActive
    );
  }
}
