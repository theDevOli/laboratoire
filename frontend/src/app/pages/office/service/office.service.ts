import { inject, Injectable, signal, Signal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IOfficeGet } from '../../../shared/api-contracts/IOfficeGet.Interface';
import { IOfficeDetails } from '../../../shared/interfaces/IOfficeDetails.interface';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { NotificationsService } from '../../../core/services/notifications.service';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IOfficeUpsert } from '../../../shared/api-contracts/IOfficeUpsert.interface';

@Injectable({
  providedIn: 'root',
})
export class OfficeService implements IService {
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _notificationService = inject(NotificationsService);

  public entities = signal<IOfficeDetails[]>([]);
  public async getEntities(): Promise<void> {
    try {
      this._loaderService.setLoading();
      const res = await this._httpService.makeRequestAsync<IOfficeGet[]>(
        'GET',
        Constants.OFFICE_END_POINT
      );
      if (!res) return;

      const tempEntities: IOfficeDetails[] = res.data
        .map(
          (office): IOfficeDetails => ({
            officeName: office.officeName,
            city: office.city,
            officeEmail: office.officeEmail,
            details: {
              officeId: office.officeId,
            },
          })
        )
        .sort((a, b) => a.officeName.localeCompare(b.officeName));

      this.entities.set(tempEntities);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      officeName: new FormControl(''),
      city: new FormControl(''),
      officeEmail: new FormControl(''),
    });
  }

  public getHeader(): string[] {
    return ['Escritório', 'Cidade', 'Email'];
  }

  public getModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Escritório',
        tabId: 'office',
        data: [
          {
            type: 'text',
            nameId: 'officeName',
            label: 'Nome do Escritório',
          },
          {
            type: 'text',
            nameId: 'city',
            label: 'Cidade',
          },
          {
            type: 'email',
            nameId: 'officeEmail',
            label: 'Email',
          },
        ],
      },
    ];
  }

  public getUpsertBodyRequest(form: FormGroup) {
    const body: IOfficeUpsert = {
      officeName: form.get('officeName')?.value,
      city: form.get('city')?.value,
      officeEmail: form.get('officeEmail')?.value,
    };

    return body;
  }

  public async makeEntityUpsertRequest(
    method: 'POST' | 'PUT',
    body: IOfficeUpsert,
    officeId: string | null = null
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const url =
        method === 'POST'
          ? Constants.OFFICE_END_POINT
          : Constants.OFFICE_END_POINT + `/${officeId}`;
      const res = await this._httpService.makeRequestAsync(method, url, body);
      if (!res?.error) this.getEntities();
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }
}
