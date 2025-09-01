import { IResult } from '../interfaces/IResult.interface';

export interface IProtocolPut {
  reportId: string;
  transactionId: number;
  cashFlowId: number;
  protocolId: string;
  results: IResult[] | null;
  catalogId: number;
  clientId: number;
  crops: number[] | null;
  entryDate: string;
  isCollectedByClient: boolean;
  partnerId: string;
  paymentDate: string;
  propertyId: number;
  reportDate: string;
  totalPaid: number | null;
}
