import { IDetails } from './IDetails.interface';

export interface IPropertyDetails extends IDetails {
  fullLocation: string;
  ccir: string | null;
  itrNirf: string | null;
  clientName: string;
  clientTaxId: string;
  details: {
    propertyId: number;
    clientId: number;
    area: string | null;
    registration: string | null;
    city: string;
    postalCode: string;
    stateId: number;
    stateCode: string;
    propertyName: string;
  };
}
