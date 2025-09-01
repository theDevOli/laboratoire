import { inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { IModalOptions } from '../../../shared/interfaces/IModalOptions.interface';
import { IPermissionGet } from '../../../shared/api-contracts/IPermissionGet.interface';
import { IPermissionPut } from '../../../shared/api-contracts/IPermissionPut.interface';
import { IPermissionPost } from '../../../shared/api-contracts/IPermissionPost.interface';
import { IPermissionDetails } from '../../../shared/interfaces/IPermissionDetails.interface';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { NotificationsService } from '../../../core/services/notifications.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionService implements IService {
  private _notificationService = inject(NotificationsService);
  private _loaderService = inject(LoaderService);
  private _httpService = inject(HttpService);
  private _permissionOptions: IModalOptions[] = [
    {
      nameId: 'r',
      label: 'Leitura',
      value: 'Leitura',
    },
    {
      nameId: 'w',
      label: 'Escrita',
      value: 'Escrita',
    },
    {
      nameId: 'n',
      label: 'Sem Permissão',
      value: 'Sem Permissão',
    },
  ];

  entities = signal<IPermissionDetails[]>([]);

  public async getEntities(): Promise<void> {
    try {
      this._loaderService.setLoading();
      const response = await this._httpService.makeRequestAsync<
        IPermissionGet[]
      >('GET', Constants.PERMISSION_END_POINT);
      if (!response) return;

      const tempPermission = response.data.map(
        (permission): IPermissionDetails => ({
          roleName: permission.roleName,
          protocol: this.getPermission(permission.protocol),
          client: this.getPermission(permission.client),
          property: this.getPermission(permission.property),
          cashFlow: this.getPermission(permission.cashFlow),
          partner: this.getPermission(permission.partner),
          users: this.getPermission(permission.users),
          chemical: this.getPermission(permission.chemical),
          details: {
            permissionId: permission.permissionId,
            roleId: permission.roleId,
          },
        })
      );

      this.entities.set(tempPermission);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getPermission(
    key: boolean | null
  ): 'Leitura' | 'Escrita' | 'Sem Permissão' {
    if (key === null) return 'Sem Permissão';
    if (!key) return 'Leitura';
    return 'Escrita';
  }

  private setPermission(
    key: any
  ): boolean | null {
    if (key === 'Escrita') return true;
    if (key === 'Leitura') return false;
    return null;
  }

  getModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Nível de Permissão',
        tabId: 'permission',
        data: [
          {
            type: 'dropdown',
            nameId: 'protocol',
            label: 'Protocolo',
            options: this._permissionOptions,
          },
          {
            type: 'dropdown',
            nameId: 'client',
            label: 'Clientes',
            options: this._permissionOptions,
          },
          {
            type: 'dropdown',
            nameId: 'property',
            label: 'Propriedades',
            options: this._permissionOptions,
          },
          {
            type: 'dropdown',
            nameId: 'cashFlow',
            label: 'Caixa',
            options: this._permissionOptions,
          },
          {
            type: 'dropdown',
            nameId: 'partner',
            label: 'Parceiros',
            options: this._permissionOptions,
          },
          {
            type: 'dropdown',
            nameId: 'chemical',
            label: 'Reagentes Quimico',
            options: this._permissionOptions,
          },
        ],
      },
    ];
  }

  getFormGroup(): FormGroup<any> {
    return new FormGroup({
      protocol: new FormControl('', Validators.required),
      client: new FormControl('', Validators.required),
      property: new FormControl('', Validators.required),
      cashFlow: new FormControl('', Validators.required),
      partner: new FormControl('', Validators.required),
      users: new FormControl('', Validators.required),
      chemical: new FormControl('', Validators.required),
    });
  }

  getHeader(): string[] {
    return [
      'Nível de Permissão',
      'Protocolo',
      'Clientes',
      'Propriedades',
      'Caixa',
      'Parceiros',
      'Usuarios',
      'Reagentes Quimico',
    ];
  }

  public getRequestBody(
    method: 'POST' | 'PUT',
    submitForm: ISubmitForm
  ): IPermissionPost | IPermissionPut {
    const { data, form } = submitForm;

    const protocol = this.setPermission(form.get('protocol')?.value);
    const client = this.setPermission(form.get('client')?.value);
    const property = this.setPermission(form.get('property')?.value);
    const cashFlow = this.setPermission(form.get('cashFlow')?.value);
    const partner = this.setPermission(form.get('partner')?.value);
    const users = this.setPermission(form.get('users')?.value);
    const chemical = this.setPermission(form.get('chemical')?.value);

    if (method === 'PUT')
      return {
        roleId: data.details.roleId,
        permissionId: data.details.permissionId,
        protocol,
        client,
        property,
        cashFlow,
        partner,
        users,
        chemical,
      };

    return {
      roleId: form.get('roleId')?.value,
      protocol,
      client,
      property,
      cashFlow,
      partner,
      users,
      chemical,
    };
  }

  public async makeEntityUpsertRequest(
    method: 'PUT' | 'POST',
    data: any
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const url =
        method === 'POST'
          ? Constants.PERMISSION_END_POINT
          : `${Constants.PERMISSION_END_POINT}/${data.permissionId}`;

      const response = await this._httpService.makeRequestAsync(
        method,
        url,
        data
      );

      if (!response?.error) this.getEntities();
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }
}
