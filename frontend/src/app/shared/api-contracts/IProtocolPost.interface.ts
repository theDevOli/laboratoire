export interface IProtocolPost {
  clientId: number;
  propertyId: number;
  partnerId: string | null;
  catalogId: number;
  entryDate: string;
  reportDate: string;
  crops: number[] | null;
  isCollectedByClient: boolean;
  transactionId: number;
  totalPaid: number | null;
  paymentDate: string | null;
}
