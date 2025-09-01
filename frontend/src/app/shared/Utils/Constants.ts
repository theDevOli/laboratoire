import { environment } from '../../../environments/environment';

export class Constants {
  static readonly TOAST_DURATION: number = 3000;
  static readonly URL_BASE: string = environment.apiUrl;

  static readonly AUTHENTICATION_END_POINT: string = `${this.URL_BASE}/v1/api/User/Authentication`;
  static readonly AUTH_END_POINT: string = `${this.URL_BASE}/v1/api/Auth`;

  static readonly CASH_FLOW_END_POINT: string = `${this.URL_BASE}/v1/api/CashFlow`;
  static readonly CATALOG_END_POINT: string = `${this.URL_BASE}/v1/api/Catalog`;
  static readonly CLIENT_END_POINT: string = `${this.URL_BASE}/v1/api/Client`;
  static readonly CROP_END_POINT: string = `${this.URL_BASE}/v1/api/Crop`;

  static readonly DISPLAY_PROTOCOL_END_POINT: string = `${this.URL_BASE}/v1/api/Protocol/DisplayProtocol`;

  static readonly GENERATE_REPORT_END_POINT: string = `${this.URL_BASE}/v1/api/Report/GenerateReport/`;

  static readonly LOGIN_END_POINT: string = `${this.URL_BASE}/v1/api/Auth/Login`;

  static readonly PARAMETER_END_POINT: string = `${this.URL_BASE}/v1/api/Parameter`;
  static readonly PARTNER_END_POINT: string = `${this.URL_BASE}/v1/api/Partner`;
  static readonly PERMISSION_END_POINT: string = `${this.URL_BASE}/v1/api/Permission`;
  static readonly PROPERTY_END_POINT: string = `${this.URL_BASE}/v1/api/Property`;
  static readonly PROTOCOL_END_POINT: string = `${this.URL_BASE}/v1/api/Protocol`;
  static readonly PROTOCOL_YEARS_END_POINT: string = `${this.URL_BASE}/v1/api/Protocol/Years`;

  static readonly TRANSACTION_YEARS_END_POINT: string = `${this.URL_BASE}/v1/api/Transaction`;
  static readonly TOKEN_VALIDATOR_END_POINT: string = `${this.URL_BASE}/v1/api/Auth/ValidateToken`;

  static readonly REFRESH_END_POINT: string = `${this.URL_BASE}/v1/api/Auth/RefreshToken`;
  static readonly REPORT_END_POINT: string = `${this.URL_BASE}/v1/api/Report`;
  static readonly ROLE_END_POINT: string = `${this.URL_BASE}/v1/api/Role`;

  static readonly USER_END_POINT: string = `${this.URL_BASE}/v1/api/User`;
  static readonly UTILS_STATES_END_POINT: string = `${this.URL_BASE}/v1/api/Utils/States`;
}
