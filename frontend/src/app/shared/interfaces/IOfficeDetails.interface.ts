import { IDetails } from './IDetails.interface';

export interface IOfficeDetails extends IDetails {
  officeName: string;
  city: string;
  officeEmail: string;
  details: {
    officeId: string;
  };
}
