import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';

import { IService } from '../../../shared/interfaces/IService.interface';
import { IUserGet } from '../../../shared/api-contracts/IUserGet.interface';
import { IUserPut } from '../../../shared/api-contracts/IUserPut.interface';
import { IUserPost } from '../../../shared/api-contracts/IUserPost.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { IUserDetails } from '../../../shared/interfaces/IUserDetails.interface';
import { SuccessMessage } from '../../../shared/Utils/SuccessMessage';
import { AppNotification } from '../../../shared/models/AppNotification.model';


@Injectable({
  providedIn: 'root',
})
export class UserService implements IService {
  private _destroyRef = inject(DestroyRef);
  private _notificationService = inject(NotificationsService);
  private _loaderService = inject(LoaderService);
  private _httpService = inject(HttpService);
  private _globalDataService = inject(GlobalDataService);

  public entities = signal<IUserDetails[]>([]);

  public async getEntities(): Promise<void> {
    try {
      this._loaderService.setLoading();

      const response = await this._httpService.makeRequestAsync<IUserGet[]>(
        'GET',
        Constants.USER_END_POINT
      );

      if (!response) return;
      const tempUser = response.data.map(
        (user): IUserDetails => ({
          name: user.name,
          username: user.username,
          roleName: user.roleName,
          isActive: user.isActive ? 'Sim' : 'Não',
          details: {
            userId: user.userId,
            partnerId: user.partnerId,
            roleId: user.roleId,
          },
        })
      );

      this.entities.set(tempUser);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  getFormGroup(): FormGroup<any> {
    return new FormGroup({
      name: new FormControl('', Validators.required),
      username: new FormControl('', Validators.required),
      roleId: new FormControl('', Validators.required),
      partnerId: new FormControl(''),
      isActive: new FormControl(true),
    });
  }

  public getPutModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Usuário',
        tabId: 'user',
        data: [
          {
            type: 'text',
            nameId: 'name',
            label: 'Nome',
            placeholder: 'Gustavo',
          },
          {
            type: 'dropdown',
            nameId: 'roleId',
            label: 'Permissão do Usuário',
            options: this._globalDataService.roleOptions(),
          },
          {
            type: 'dropdown',
            nameId: 'partnerId',
            label: 'Parceiro',
            options: this._globalDataService.partnerOptions(),
          },
          {
            type: 'checkbox',
            nameId: 'isActive',
            label: 'Está Ativo?',
          },
        ],
      },
    ];
  }

  public getPostModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Usuário',
        tabId: 'user',
        data: [
          {
            type: 'text',
            nameId: 'name',
            label: 'Nome',
            placeholder: 'Gustavo',
          },
          {
            type: 'text',
            nameId: 'username',
            label: 'Nome do Usuário',
            placeholder: 'gustavo',
          },
          {
            type: 'dropdown',
            nameId: 'roleId',
            label: 'Permissão do Usuário',
            options: this._globalDataService.roleOptions(),
          },
          {
            type: 'dropdown',
            nameId: 'partnerId',
            label: 'Parceiro',
            options: this._globalDataService.partnerOptions(),
          },
          {
            type: 'checkbox',
            nameId: 'isActive',
            label: 'Está Ativo?',
          },
        ],
      },
    ];
  }

  public getHeader(): string[] {
    return ['Nome', 'Nome do Usuário', 'Nível de Permissão', 'Está Ativo?'];
  }

  public getRequestBody(
    submitForm: ISubmitForm,
    method: 'POST' | 'PUT'
  ): IUserPut | IUserPost {
    const { form, data } = submitForm;

    const roleId = form.get('roleId')?.value;
    const partnerId = form.get('partnerId')?.value || null;
    const name = form.get('name')?.value;
    const isActive = form.get('isActive')?.value ? true : false;
    let username = '';

    if (method === 'PUT') {
      username = data.username;
      const userId = data.details.userId;
      return {
        userId,
        roleId,
        partnerId,
        username,
        name,
        isActive,
      };
    }

    username = form.get('username')?.value;
    return {
      roleId,
      partnerId,
      username,
      name,
      isActive,
    };
  }

  async makeEntityUpsertRequest(
    method: 'PUT' | 'POST',
    data: any
  ): Promise<void> {
    try {
      this._loaderService.setLoading();

      const url =
        method === 'PUT'
          ? `${Constants.USER_END_POINT}/${data.userId}`
          : `${Constants.USER_END_POINT}`;

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

  public disabledEnableControlByRoleId(form: FormGroup): void {
    const catalogIdSubscription = form
      .get('roleId')
      ?.valueChanges.subscribe((value) => {
        const roleId = Number(value);
        const partnerForm = form.get('partnerId');

        if (roleId === 0) return;
        const isEnable = roleId === 4;

        if (isEnable) {
          partnerForm?.enable();
          return;
        }
        partnerForm?.disable();
        partnerForm?.setValue('');
      });

    this._destroyRef.onDestroy(() => {
      catalogIdSubscription?.unsubscribe();
    });
  }

  public async resetPassword(userId: string): Promise<void> {
    try {
      this._loaderService.setLoading();
      const body = { userId };
      const response = await this._httpService.makeRequestAsync(
        'POST',
        `${Constants.AUTH_END_POINT}/ResetPassword`,
        body
      );
      if (!response?.error)
        this._notificationService.openNotification(
          new AppNotification(SuccessMessage.resetPassword, 'success')
        );
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }
}
