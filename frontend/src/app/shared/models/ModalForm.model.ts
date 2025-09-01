import { IModalForm } from "../interfaces/IModalForm.interface";
import { IModelFormData } from "../interfaces/IModalFormData.interface";

export class ModalForm implements IModalForm {
  constructor(
    private _tabName: string,
    private _tabId: string,
    private _data: IModelFormData[]
  ) {}

  public get tabName() {
    return this._tabName;
  }
  public get tabId() {
    return this._tabId;
  }
  public get data() {
    return this._data;
  }
}
