export interface IProtocolGet {
  cashFlowId: number;
  clientId: number;
  crops: number[];
  entryDate: string;
  isCollectedByClient: boolean;
  parameterId: string;
  partnerId: string | null;
  propertyId: string;
  protocolId: string;
  reportDate: string;
  reportId?: string | null;
}
