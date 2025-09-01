import { IResult } from "../interfaces/IResult.interface";

export interface IReportPost {
  reportId?: string;
  protocolId: string;
  results: IResult[];
}
