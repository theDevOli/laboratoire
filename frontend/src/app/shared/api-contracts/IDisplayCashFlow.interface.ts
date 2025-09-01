import { ICashFlowGet } from './ICashFlowGet.interface';

export interface IDisplayCashFlow {
  cashFlow: ICashFlowGet[];
  totalAmount: number;
}
