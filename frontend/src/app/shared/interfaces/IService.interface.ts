import { FormGroup } from '@angular/forms';
import { Signal } from '@angular/core';

import { IModalForm } from './IModalForm.interface';
import { IDetails } from './IDetails.interface';

export interface IService {
  entities: Signal<IDetails[]>;
  // getEntities(): Promise<void>
  // getPostModalForm(): IModalForm[];
  // getPutModalForm(): IModalForm[];
  getFormGroup(): FormGroup<any>;
  getHeader(): string[];
  // makeEntityUpsertRequest(method: 'PUT' | 'POST', data: any): Promise<void>;
  // controlFormatter(form: FormGroup): void;
  // disabledEnableControl(form: FormGroup): void;
}
