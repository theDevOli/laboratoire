import { IAuthenticationGet } from '../api-contracts/IAuthenticationGet.interface';
import { IPermissionGet } from '../api-contracts/IPermissionGet.interface';

export class Permission {
  constructor(
    private _protocol: boolean | null,
    private _client: boolean | null,
    private _property: boolean | null,
    private _cashFlow: boolean | null,
    private _partner: boolean | null,
    private _users: boolean | null,
    private _chemical: boolean | null
  ) {}

  public get protocol(): boolean | null {
    return this._protocol;
  }

  public set protocol(protocol: boolean | null) {
    this._protocol = protocol;
  }

  public get client(): boolean | null {
    return this._client;
  }

  public set client(client: boolean | null) {
    this._client = client;
  }

  public get property(): boolean | null {
    return this._property;
  }

  public set property(property: boolean | null) {
    this._property = property;
  }

  public get cashFlow(): boolean | null {
    return this._cashFlow;
  }

  public set cashFlow(cashFlow: boolean | null) {
    this._cashFlow = cashFlow;
  }

  public get partner(): boolean | null {
    return this._partner;
  }

  public set partner(partner: boolean | null) {
    this._partner = partner;
  }

  public get users(): boolean | null {
    return this._users;
  }

  public set users(users: boolean | null) {
    this._users = users;
  }

  public get chemical(): boolean | null {
    return this._chemical;
  }

  public set chemical(chemical: boolean | null) {
    this._chemical = chemical;
  }

  public static fromInterface(authentication: IAuthenticationGet): Permission {
    return new Permission(
      authentication.protocol,
      authentication.client,
      authentication.property,
      authentication.cashFlow,
      authentication.partner,
      authentication.users,
      authentication.chemical
    );
  }
}
