export interface IPropertyDisplayGet {
  propertyId: number;
  clientId: number;
  clientName: string;
  clientTaxId: string;
  area: string | null;
  registration:string;
  ccir: string | null;
  itrNirf: string | null;
  city: string;
  postalCode: string;
  stateId: number;
  stateCode: string;
  propertyName: string;
}
