export interface ICashFlowDetails {
  description: string;
  totalPaid: string;
  paymentDate: string;
  details: {
    cashFlowId: number;
    totalPaid: number;
    transactionId: number;
  };
}
