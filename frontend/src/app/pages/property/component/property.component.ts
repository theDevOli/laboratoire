import {
  Component,
  effect,
  inject,
  OnInit,
  WritableSignal,
} from '@angular/core';
import { FormGroup } from '@angular/forms';

import { TableComponent } from '../../../core/shell/table/table.component';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { AuthenticationService } from '../../../core/services/authentication.service';

import { PropertyService } from '../service/property.service';
import { ProtocolService } from '../../protocol/service/protocol.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IPropertyDetails } from '../../../shared/interfaces/IPropertyDetails.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';

@Component({
  selector: 'app-property',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './property.component.html',
})
export class PropertyComponent implements IComponent, OnInit {
  private _propertyService = inject(PropertyService);
  private _protocolService = inject(ProtocolService);
  private _globalDataService = inject(GlobalDataService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'PUT' | 'POST';

  public datum: WritableSignal<IPropertyDetails[]> =
    this._propertyService.entities;
  public header: string[] = this._propertyService.getHeader();
  public actions: IAction[] = [];
  public title: string = 'Propriedades';
  public newRecord: string = 'Nova Propriedade';
  public modalForm: IModalForm[] = this._propertyService.propertyModalForm();
  public form: FormGroup<any> = this._propertyService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.property;

  constructor() {
    effect(() => {
      this.modalForm = this._propertyService.propertyModalForm();
    });
  }

  public ngOnInit(): void {
    this._propertyService.getEntities();

    this._propertyService.setValidatorsOnChange(this.form);

    this._propertyService.disabledEnableControl(this.form);

    this._propertyService.controlFormatter(this.form);
  }

  public onSubmitForm = (submitForm: ISubmitForm): void => {
    const { form, data } = submitForm;

    if (!form.touched) return;

    if (form.get('toPostProtocol')?.value) {
      const clientId = form.get('clientId')?.value;
      const propertyId = data.details.propertyId;

      const protocolBody = this._protocolService.getProtocolPostBody(
        form,
        clientId,
        propertyId
      );

      this._protocolService.makeEntityInsertRequest(protocolBody);
      return;
    }

    const clientId = form.get('clientId')?.value;
    const body = this._propertyService.getUpsertBodyRequest(
      form,
      data,
      this._method,
      clientId
    );

    this._propertyService.makeEntityUpsertRequest(this._method, body);
  };

  public async onSetForm(formData: ISetForm): Promise<void> {
    const { method, toUpdateData } = formData;

    this._globalDataService.setInitChips([]);

    this._method = method;

    // await this._globalDataService.getClientOptions();

    if (method === 'POST') {
      this._propertyService.setPostValidators(this.form);
      // this.modalForm = this._propertyService.getPostModalFormWithClient();
      this._propertyService.resetPOSTModalForm();
      return;
    }

    // this.modalForm = this._propertyService.getPutModalForm();
    this._propertyService.resetPUTModalForm();
    this._propertyService.setPutValidators(this.form);

    this.form.patchValue({
      ...toUpdateData,
      ...this._protocolService.getDate(),
    });
  }
}
