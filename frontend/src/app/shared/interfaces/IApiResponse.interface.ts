import { IError } from "./IError.interface";

export interface IApiResponse {
  data: any;
  error: IError | null;
}
