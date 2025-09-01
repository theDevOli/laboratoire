import { WritableSignal } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { IAction } from './IAction.interface';
import { IModalForm } from './IModalForm.interface';
import { ISetForm } from './ISetForm.interface';
import { ISubmitForm } from './ISubmitForm.interface';

export interface IComponent {
  // method: 'PUT' | 'POST';
  datum: WritableSignal<any[]>;
  header: string[];
  actions: IAction[];
  title: string;
  newRecord: string | null;
  modalForm: IModalForm[];
  form: FormGroup<any>;
  permission: boolean | null | undefined;
  // ngOnInit(): void | Promise<void>;
  onSubmitForm(submitForm: ISubmitForm): void | Promise<void>;
  onSetForm(formData: ISetForm): void | Promise<void>;
}
