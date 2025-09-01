import { Component, inject, OnInit, WritableSignal } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { TableComponent } from '../../../core/shell/table/table.component';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';
import { AuthenticationService } from '../../../core/services/authentication.service';

import { ClientService } from '../service/client.service';
import { PropertyService } from '../../property/service/property.service';
import { ProtocolService } from '../../protocol/service/protocol.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { IClientDetails } from '../../../shared/interfaces/IClientDetails.interface';
import { SuccessMessage } from '../../../shared/Utils/SuccessMessage';
import { AppNotification } from '../../../shared/models/AppNotification.model';

@Component({
  selector: 'app-client',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './client.component.html',
})
export class ClientComponent implements IComponent, OnInit {
  private _clientService = inject(ClientService);
  private _propertyService = inject(PropertyService);
  private _protocolService = inject(ProtocolService);
  private _globalDataService = inject(GlobalDataService);
  private _notificationService = inject(NotificationsService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'PUT' | 'POST';

  public datum: WritableSignal<IClientDetails[]> = this._clientService.entities;
  public header: string[] = this._clientService.getHeader();
  public actions: IAction[] = [];
  public title: string = 'Clientes';
  public newRecord: string = 'Novo Cliente';
  public modalForm: IModalForm[] = this._clientService.getPutModalForm();
  public form: FormGroup<any> = this._clientService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.client;

  ngOnInit(): void {
    this._clientService.getEntities();

    this._clientService.setValidatorsOnChange(this.form);

    this._clientService.controlFormatter(this.form);
    this._propertyService.controlFormatter(this.form);

    this._clientService.disabledEnableControl(this.form);
  }

  public onSubmitForm = async (submitForm: ISubmitForm): Promise<void> => {
    const { form, data } = submitForm;
    if (!form.touched) return;
    const clientId = data?.details?.clientId;
    const toPostProtocol = form.get('toPostProtocol')?.value;
    const toPostProperty = form.get('toPostProperty')?.value;

    if (!toPostProperty && !toPostProtocol) {
      const body = this._clientService.getUpsertBodyRequest(
        form,
        this._method,
        clientId
      );

      this._clientService.makeEntityUpsertRequest(this._method, body);
    }

    if (toPostProperty) {
      const propertyBody = this._propertyService.getUpsertBodyRequest(
        form,
        data,
        'POST',
        clientId
      );

      await this._propertyService.makeEntityUpsertRequest('POST', propertyBody);
      this._notificationService.openNotification(
        new AppNotification(SuccessMessage.propertyAdded, 'success')
      );
    }

    if (toPostProtocol) {
      const propertyId = form.get('propertyId')?.value;
      const protocolBody = this._protocolService.getProtocolPostBody(
        form,
        clientId,
        propertyId
      );

      this._protocolService.makeEntityInsertRequest(protocolBody);
    }
  };

  public async onSetForm(formData: ISetForm): Promise<void> {
    const { method, data, toUpdateData } = formData;

    this._globalDataService.setInitChips([]);

    this._method = method;

    if (method === 'POST') {
      this.modalForm = this._clientService.getPostModalForm();
      this._clientService.setPostValidators(this.form);
      return;
    }

    await this._globalDataService.getPropertyOptions(data?.details?.clientId);
    this.modalForm = this._clientService.getPutModalForm();
    this._clientService.setPutValidators(this.form);

    this.form.patchValue({
      ...toUpdateData,
      ...this._protocolService.getDate(),
    });
  }
}
