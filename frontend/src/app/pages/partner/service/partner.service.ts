import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Utils } from '../../../shared/Utils/Utils';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IPartnerGet } from '../../../shared/api-contracts/IPartnerGet.interface';
import { IPartnerDetails } from '../../../shared/interfaces/IPartnerDetails.interface';

import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { HttpService } from '../../../core/services/http.service';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { IPartnerPost } from '../../../shared/api-contracts/IPartnerPost.interface';
import { LoaderService } from '../../../core/services/loader.service';
import { GlobalService } from '../../../core/services/global.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';

@Injectable({
  providedIn: 'root',
})
export class PartnerService implements IService {
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _globalDataService = inject(GlobalDataService);
  private _globalService = inject(GlobalService);
  private _notificationService = inject(NotificationsService);
  private _destroyRef = inject(DestroyRef);

  public entities = signal<IPartnerDetails[]>([]);

  public async getEntities(): Promise<any> {
    try {
      this._loaderService.setLoading();
      const res = await this._httpService.makeRequestAsync<IPartnerGet[]>(
        'GET',
        Constants.PARTNER_END_POINT
      );
      if (!res) return;

      const tempPartner = res.data.map(
        (partner): IPartnerDetails => ({
          officeName: partner.officeName,
          partnerEmail: partner.partnerEmail,
          partnerName: partner.partnerName,
          partnerPhone: Utils.phoneFormatter(partner.partnerPhone),
          // active: true,
          details: {
            partnerId: partner.partnerId,
          },
        })
      );

      this.entities.set(tempPartner);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getPutModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Parceiro',
        tabId: 'partner',
        data: [
          {
            type: 'text',
            nameId: 'officeName',
            label: 'Escritório',
            placeholder: 'Escritório AgrPec',
          },
          {
            type: 'text',
            nameId: 'partnerEmail',
            label: 'Email',
            placeholder: 'escritorio@email.com',
          },
          {
            type: 'text',
            nameId: 'partnerName',
            label: 'Nome do Parceiro',
            placeholder: 'Tiago Santos',
          },
          {
            type: 'text',
            nameId: 'partnerPhone',
            label: 'Contato',
            placeholder: Utils.phoneFormatter('79999998877'),
          },
        ],
      },
    ];
  }
  public getPostModalForm(): IModalForm[] {
    return [
      ...this.getPutModalForm(),
      {
        tabName: 'Usuário',
        tabId: 'user',
        data: [
          {
            type: 'checkbox',
            nameId: 'isActive',
            label: 'Ativo?',
          },
          {
            type: 'text',
            nameId: 'username',
            label: 'Nome do Usuário',
            placeholder: 'gelvane',
          },
        ],
      },
    ];
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      officeName: new FormControl('', Validators.required),
      partnerEmail: new FormControl('', [
        Validators.required,
        Validators.email,
      ]),
      partnerName: new FormControl('', Validators.required),
      partnerPhone: new FormControl('', [
        Validators.required,
        Validators.minLength(16),
      ]),
      //Related to user
      username: new FormControl(''),
      isActive: new FormControl(true),
    });
  }

  getHeader(): string[] {
    return ['Escritório', 'Email', 'Nome do Parceiro', 'Contato'];
  }

  public getRequestBody(
    method: 'PUT' | 'POST',
    submitForm: ISubmitForm
  ): IPartnerGet | IPartnerPost {
    const { form, data } = submitForm;
    const officeName = form.get('officeName')?.value || '';
    const partnerEmail = form.get('partnerEmail')?.value || '';
    const partnerName = form.get('partnerName')?.value || '';
    const tempPartnerPhone = form.get('partnerPhone')?.value || '';
    const partnerPhone = Utils.phoneFormatter(tempPartnerPhone, true);
    const username = form.get('username')?.value || '';
    const isActive = form.get('isActive')?.value || false;

    if (method === 'POST')
      return {
        officeName,
        partnerEmail,
        partnerName,
        partnerPhone,
        roleId: 4,
        username,
        isActive,
      };

    return {
      officeName,
      partnerEmail,
      partnerId: data.details.partnerId,
      partnerName,
      partnerPhone,
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
          ? Constants.PARTNER_END_POINT
          : `${Constants.PARTNER_END_POINT}/${data.partnerId}`;
      const res = await this._httpService.makeRequestAsync(method, url, data);

      if (!res || res.error) return;

      this.getEntities();
      this._globalDataService.cacheData();
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public setPostValidators(form: FormGroup): void {
    const setValidators = [
      // 'officeName',
      // 'partnerName',
      // 'partnerEmail',
      // 'partnerPhone',
      'username',
    ];

    this._globalService.setValidator(form, setValidators);
  }

  public setPutValidators(form: FormGroup): void {
    // const setValidators = [
    //   'officeName',
    //   'partnerEmail',
    //   'partnerName',
    //   'partnerPhone',
    // ];
    const removeValidators = ['username'];

    this._globalService.removeValidator(form, removeValidators);
    // this._globalService.setValidator(form, setValidators);
  }

  public controlFormatter(form: FormGroup): void {
    const phoneSubscription = form
      .get('partnerPhone')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.phoneFormatter(value);
        form.get('partnerPhone')?.setValue(formatted, { emitEvent: false });
      });

    this._destroyRef.onDestroy(() => {
      phoneSubscription?.unsubscribe();
    });
  }
}
