import { IDetails } from './IDetails.interface';

export interface IClientDetails extends IDetails {
  clientName: string;
  clientTaxId: string;
  clientEmail: string;
  clientPhone: string;
  details: {
    clientId: string;
  };
}
