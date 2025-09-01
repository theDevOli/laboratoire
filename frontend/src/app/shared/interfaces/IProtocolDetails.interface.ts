import { IDetails } from './IDetails.interface';

export interface IProtocolDetails extends IDetails {
  protocolId: string;
  clientName: string;
  clientTaxId: string;
  fullLocation: string;
  partnerName: string | null;
  isPaid: 'Sim' | 'Não';
  // reportType: string;
  details: {
    totalPaid: number | null;
    entryDate: string;
    city: string;
    cashFlowId: number;
    postalCode: string;
    stateId: number;
    stateCode: string;
    isCollectedByClient: boolean;
    location: string;
    area: string;
    ccir: string;
    itrNirf: string;
    propertyName: string;
    paymentDate: string;
    price: number;
    reportId: string;
    partnerId: string | null;
    reportDate: string;
    results: object[];
    crops?: number[];
    catalogId: number;
    clientId: number;
    propertyId: number;
    transactionId: number;
  };
}
