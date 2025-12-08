import { Component, inject, OnInit, WritableSignal } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { OfficeService } from '../service/office.service';

import { AuthenticationService } from '../../../core/services/authentication.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';

@Component({
  selector: 'app-office',
  standalone: true,
  imports: [],
  templateUrl: './office.component.html',
})
export class OfficeComponent implements IComponent, OnInit {
  private _officeService = inject(OfficeService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'PUT' | 'POST';

  public datum: WritableSignal<any[]> = this._officeService.entities;
  public header: string[] = this._officeService.getHeader();
  public actions: IAction[] = [];
  public title: string = 'Escritórios';
  public newRecord: string = 'Novo Escritório';
  public modalForm: IModalForm[] = this._officeService.getModalForm();
  public form: FormGroup<any> = this._officeService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.client;

  public ngOnInit(): void {
    this._officeService.getEntities();
  }

  public async onSubmitForm(submitForm: ISubmitForm): Promise<void> {
    const {form, data} = submitForm;
    const officeId = data?.details?.officeId || null;

    const body = this._officeService.getUpsertBodyRequest(form);

    this._officeService.makeEntityUpsertRequest(this._method, body, officeId);
  }

  public onSetForm(formData: ISetForm): void {
    const { method, toUpdateData } = formData;

    this._method = method;

    if (method === 'PUT') {
      this.form.patchValue({ ...toUpdateData });
      return;
    }

    this.form.reset();
  }
}
