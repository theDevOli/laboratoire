import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Utils } from '../../../shared/Utils/Utils';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IOfficeGet } from '../../../shared/api-contracts/IOfficeGet.Interface';
import { IPartnerGet } from '../../../shared/api-contracts/IPartnerGet.interface';
import { IModalOptions } from '../../../shared/interfaces/IModalOptions.interface';
import { IPartnerUpsert } from '../../../shared/api-contracts/IPartnerUpsert.interface';
import { IPartnerDetails } from '../../../shared/interfaces/IPartnerDetails.interface';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';

@Injectable({
  providedIn: 'root',
})
export class PartnerService implements IService {
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _globalDataService = inject(GlobalDataService);
  private _notificationService = inject(NotificationsService);
  private _destroyRef = inject(DestroyRef);

  public entities = signal<IPartnerDetails[]>([]);

  private _officeIdOptions = signal<IModalOptions[]>([]);

  public async getEntities(): Promise<any> {
    try {
      this._loaderService.setLoading();
      const [partnerRes, officeRes] = await Promise.all([
        this._httpService.makeRequestAsync<IPartnerGet[]>(
          'GET',
          Constants.PARTNER_END_POINT
        ),
        this._httpService.makeRequestAsync<IOfficeGet[]>(
          'GET',
          Constants.OFFICE_END_POINT
        ),
      ]);
      if (!partnerRes || !officeRes) return;

      const tempOption: IModalOptions[] = officeRes.data
      .map((office) => ({
        nameId: office.officeId,
        label: office.officeName,
        value: office.officeId,
      }))
      .sort((a, b) => a.label.localeCompare(b.label));

      const tempPartner = partnerRes.data.map((partner): IPartnerDetails => {
        const officeName =
          officeRes.data.find((office) => office.officeId === partner.officeId)
            ?.officeName ?? '';
        return {
          officeName,
          partnerName: partner.partnerName,
          partnerPhone: Utils.phoneFormatter(partner.partnerPhone),
          details: {
            partnerId: partner.partnerId,
            officeId: partner.officeId,
          },
        };
      });

      this.entities.set(tempPartner);

      this._officeIdOptions.set(tempOption);
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
            type: 'dropdown',
            nameId: 'officeId',
            label: 'Escritório',
            placeholder: 'Escritório AgrPec',
            options: this._officeIdOptions(),
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
    return [...this.getPutModalForm()];
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      officeId: new FormControl('', Validators.required),
      partnerName: new FormControl('', Validators.required),
      partnerPhone: new FormControl('', [
        Validators.required,
        Validators.minLength(16),
      ]),
    });
  }

  getHeader(): string[] {
    return ['Escritório', 'Nome do Parceiro', 'Contato'];
  }

  public getRequestBody(form: FormGroup<any>): IPartnerUpsert {
    const officeId = form.get('officeId')?.value || '';
    const partnerName = form.get('partnerName')?.value || '';
    const tempPartnerPhone = form.get('partnerPhone')?.value || '';
    const partnerPhone = Utils.phoneFormatter(tempPartnerPhone, true);

    return {
      officeId,
      partnerName,
      partnerPhone,
    };
  }

  public async makeEntityUpsertRequest(
    method: 'PUT' | 'POST',
    data: IPartnerUpsert,
    partnerId:string
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const url =
        method === 'POST'
          ? Constants.PARTNER_END_POINT
          : `${Constants.PARTNER_END_POINT}/${partnerId}`;
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
