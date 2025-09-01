import { IModalOptions } from './IModalOptions.interface';

export interface IModelFormData {
  type:
    | 'text'
    | 'email'
    | 'password'
    | ''
    | 'number'
    | 'date'
    | 'textArea'
    | 'checkbox'
    | 'radio'
    | 'dropdown'
    | 'chips';
  nameIdB?: string;
  nameIdC?: string;
  nameId: string;
  label: string;
  min?: number;
  placeholder?: string;
  rows?: number;
  options?: IModalOptions[];
}
