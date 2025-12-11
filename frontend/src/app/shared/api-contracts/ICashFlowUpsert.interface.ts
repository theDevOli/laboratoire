export interface ICashFlowUpsert {
  description: string;
  transactionId: number;
  totalPaid: number;
  paymentDate: string;
  partnerId?: number;
}
