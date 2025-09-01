export interface IPropertyPut {
  propertyId: number;
  clientId: number;
  stateId: number;
  propertyName: string;
  city: string;
  postalCode: string | null;
  area: string | null;
  registration: string | null;
  ccir: string | null;
  itrNirf: string | null;
}
