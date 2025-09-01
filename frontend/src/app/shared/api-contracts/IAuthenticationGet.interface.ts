export interface IAuthenticationGet {
  username: string;
  partnerId: string | null;
  clientId: string | null;
  name: string;
  isActive: boolean;
  protocol: boolean | null;
  client: boolean | null;
  property: boolean | null;
  cashFlow: boolean | null;
  partner: boolean | null;
  users: boolean | null;
  chemical: boolean | null;
}
