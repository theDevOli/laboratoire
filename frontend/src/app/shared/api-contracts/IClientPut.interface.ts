export interface IClientPut{
  clientId: string;
  userId: string;
  clientName: string;
  clientTaxId: string;
  clientEmail: string | null;
  clientPhone: string | null;
}
