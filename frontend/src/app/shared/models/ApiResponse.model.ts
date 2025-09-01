import { IError } from "../interfaces/IError.interface";
import { IApiResponse } from "../interfaces/IApiResponse.interface";

export class ApiResponse<T> implements IApiResponse{
  private _data: T;
  private _error: IError | null;

  constructor(data: T, error: IError | null) {
    this._data = data;
    this._error = error;
  }

  get data(): T {
    return this._data;
  }

  get error(): IError | null {
    return this._error;
  }
}
