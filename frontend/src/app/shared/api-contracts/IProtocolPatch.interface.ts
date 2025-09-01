export interface IProtocolPatch {
  protocolId: string;
  entryDate: string;
  isCollectedByClient: boolean;
  partnerId: string | null;
  reportDate: string;
}
