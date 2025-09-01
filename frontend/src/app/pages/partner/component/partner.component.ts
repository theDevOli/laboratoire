import { Component, inject, OnInit, WritableSignal } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { PartnerService } from '../service/partner.service';

import { TableComponent } from '../../../core/shell/table/table.component';
import { AuthenticationService } from '../../../core/services/authentication.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';

@Component({
  selector: 'app-partner',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './partner.component.html',
})
export class PartnerComponent implements IComponent, OnInit {
  private _partnerService = inject(PartnerService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'POST' | 'PUT';

  public datum: WritableSignal<any[]> = this._partnerService.entities;
  public header: string[] = this._partnerService.getHeader();
  public actions: IAction[] = [];
  public title = 'Parceiros';
  public newRecord = 'Novo Parceiro';
  public modalForm: IModalForm[] = this._partnerService.getPutModalForm();
  public form: FormGroup<any> = this._partnerService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.partner;

  async ngOnInit(): Promise<void> {
    this._partnerService.getEntities();
    this._partnerService.controlFormatter(this.form);
  }

  onSubmitForm = (submitForm: ISubmitForm): void => {
    const body = this._partnerService.getRequestBody(this._method, submitForm);

     this._partnerService.makeEntityUpsertRequest(this._method, body);
  };

  public onSetForm(formData: ISetForm): void{
    const { method, toUpdateData } = formData;
    this._method = method;
    this.form.reset();

    if (method === 'POST') {
      this.modalForm = this._partnerService.getPostModalForm();
      this._partnerService.setPostValidators(this.form);
      return;
    }

    this.modalForm = this._partnerService.getPutModalForm();
    this._partnerService.setPutValidators(this.form);

    this.form.patchValue(toUpdateData);
  }
}
