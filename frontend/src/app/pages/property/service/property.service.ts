import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DestroyRef, inject, Injectable, signal } from '@angular/core';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';

import { ProtocolService } from '../../protocol/service/protocol.service';

import { Utils } from '../../../shared/Utils/Utils';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IPropertyPut } from '../../../shared/api-contracts/IPropertyPut.interface';
import { IPropertyPost } from '../../../shared/api-contracts/IPropertyPost.interface';
import { GlobalService } from '../../../core/services/global.service';
import { IPropertyDetails } from '../../../shared/interfaces/IPropertyDetails.interface';
import { IPropertyDisplayGet } from '../../../shared/api-contracts/IPropertyDisplayGet.interface';

@Injectable({
  providedIn: 'root',
})
export class PropertyService implements IService {
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _notificationService = inject(NotificationsService);
  private _globalDataService = inject(GlobalDataService);
  private _protocolService = inject(ProtocolService);
  private _destroyRef = inject(DestroyRef);
  private _globalService = inject(GlobalService);

  public entities = signal<IPropertyDetails[]>([]);
  public propertyModalForm = signal<IModalForm[]>(this.getPutModalForm());

  public getHeader(): string[] {
    return ['Nome do Cliente', 'CPF/CNPJ', 'Localização', 'CCIR', 'ITR/NIRF'];
  }

  public async getEntities(): Promise<void> {
    try {
      this._loaderService.setLoading();
      const response = await this._httpService.makeRequestAsync<
        IPropertyDisplayGet[]
      >('GET', Constants.PROPERTY_END_POINT + `/Display`);

      if (!response || !response.data) return;

      const propertyDisplay = response.data.map(
        (property): IPropertyDetails => {
          const fullLocation =
            `${property.city} - ${property.stateCode}, ${
              property.propertyName ?? ''
            }` + (property.area ? ` (${property.area})` : '');

          return {
            clientName: property.clientName,
            clientTaxId: Utils.taxFormatter(property.clientTaxId),
            fullLocation: fullLocation,
            ccir: property.ccir ? Utils.ccirFormatter(property.ccir) : '-',
            itrNirf: property.itrNirf
              ? Utils.itrNirfFormatter(property.itrNirf)
              : '-',
            details: {
              propertyId: property.propertyId,
              clientId: property.clientId,
              area: property.area,
              registration: property.registration,
              propertyName: property.propertyName,
              city: property.city,
              postalCode: property.postalCode,
              stateId: property.stateId,
              stateCode: property.stateCode,
            },
          };
        }
      );
      this.entities.set(propertyDisplay);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getPostModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Propriedade',
        tabId: 'property',
        data: [
          {
            type: 'text',
            nameId: 'area',
            label: 'Área',
            placeholder: ' 00 ha',
          },
          {
            type: 'text',
            nameId: 'registration',
            label: 'Matrícula',
            placeholder: ' 2.478',
          },
          {
            type: 'text',
            nameId: 'city',
            label: 'Cidade',
            placeholder: 'Itabaiana',
          },
          {
            type: 'text',
            nameId: 'postalCode',
            label: 'CEP',
            placeholder: '49.500-000',
          },
          {
            type: 'dropdown',
            nameId: 'stateId',
            label: 'Estado',
            options: this._globalDataService.stateOptions(),
          },
          {
            type: 'text',
            nameId: 'propertyName',
            label: 'Nome do Imóvel',
            placeholder: 'Fazenda Guaraná',
          },
          {
            type: 'text',
            nameId: 'ccir',
            label: 'CCIR',
            placeholder: '000.000.000.000-0',
          },
          {
            type: 'text',
            nameId: 'itrNirf',
            label: 'ITR/NIRF',
            placeholder: '0.000.000-0',
          },
        ],
      },
    ];
  }

  public getPostModalFormWithClient(): IModalForm[] {
    const postModalForm = this.getPostModalForm();

    return [
      ...postModalForm,
      {
        tabName: 'Cliente',
        tabId: 'client',
        data: [
          {
            type: 'text',
            nameId: 'clientTaxId',
            label: 'CPF/CNPJ',
            placeholder: '000.000.000-00',
          },
          {
            type: 'dropdown',
            nameId: 'clientId',
            label: 'Cliente',
            options: this._globalDataService.clientOptions(),
          },
        ],
      },
    ];
  }

  public getPutModalForm(): IModalForm[] {
    const postPropertyModalForm = this.getPostModalForm();
    const postProtocolModalForm = this._protocolService.getPostModalForm();
    return [...postPropertyModalForm, ...postProtocolModalForm];
  }

  public setPostValidators(form: FormGroup): void {
    const setValidators = ['clientId', 'city', 'stateId', 'propertyName'];
    const removeValidators = ['catalogId', 'entryDate', 'transactionId'];

    this._globalService.setValidator(form, setValidators);
    this._globalService.removeValidator(form, removeValidators);
  }

  public setPutValidators(form: FormGroup): void {
    const setValidators = ['catalogId', 'entryDate', 'transactionId'];
    const removeValidators = ['clientId', 'city', 'stateId', 'propertyName'];

    this._globalService.setValidator(form, setValidators);
    this._globalService.removeValidator(form, removeValidators);
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      clientId: new FormControl(''),
      clientTaxId: new FormControl(''),
      area: new FormControl(''),
      city: new FormControl(''),
      postalCode: new FormControl('', Validators.minLength(10)),
      stateId: new FormControl(''),
      registration: new FormControl(''),
      propertyName: new FormControl(''),
      ccir: new FormControl(''),
      itrNirf: new FormControl(''),
      // Related to protocol
      toPostProtocol: new FormControl(false),
      partnerId: new FormControl(''),
      catalogId: new FormControl(''),
      entryDate: new FormControl(''),
      reportDate: new FormControl(''),
      crops: new FormControl(''),
      isCollectedByClient: new FormControl(true),
      transactionId: new FormControl(''),
      totalPaid: new FormControl(0),
      paymentDate: new FormControl(''),
    });
  }

  public async makeEntityUpsertRequest(
    method: 'PUT' | 'POST',
    data: IPropertyPost | IPropertyPut
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const url =
        method === 'POST'
          ? Constants.PROPERTY_END_POINT
          : Constants.PROPERTY_END_POINT +
            `/${(data as IPropertyPut).propertyId}`;

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

  public getUpsertBodyRequest(
    form: FormGroup,
    data: IPropertyDetails,
    method: 'POST' | 'PUT',
    clientId: string
  ): IPropertyPost | IPropertyPut {
    const stateId = Number(form.get('stateId')?.value);
    const propertyName = form.get('propertyName')?.value;
    const city = form.get('city')?.value;
    const postalCode =
      Utils.postalCodeFormatter(form.get('postalCode')?.value, true) || null;
    const registration = form.get('registration')?.value || null;
    const area = form.get('area')?.value;
    const ccir = Utils.ccirFormatter(form.get('ccir')?.value, true) || null;
    const itrNirf =
      Utils.itrNirfFormatter(form.get('itrNirf')?.value, true) || null;

    const body: IPropertyPost | IPropertyPut =
      method === 'POST'
        ? {
            clientId,
            stateId,
            propertyName,
            city,
            postalCode,
            registration,
            area,
            ccir,
            itrNirf,
          }
        : {
            propertyId: data.details.propertyId,
            clientId: clientId,
            stateId,
            propertyName,
            city,
            postalCode,
            registration,
            area,
            ccir,
            itrNirf,
          };

    return body;
  }

  public controlFormatter(form: FormGroup): void {
    const postalCodeSubscription = form
      .get('postalCode')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.postalCodeFormatter(value);
        form.get('postalCode')?.setValue(formatted, { emitEvent: false });
      });

    const ccirSubscription = form
      .get('ccir')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.ccirFormatter(value);
        form.get('ccir')?.setValue(formatted, { emitEvent: false });
      });

    const itrNirfSubscription = form
      .get('itrNirf')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.itrNirfFormatter(value);
        form.get('itrNirf')?.setValue(formatted, { emitEvent: false });
      });

    this._destroyRef.onDestroy(() => {
      postalCodeSubscription?.unsubscribe();
      ccirSubscription?.unsubscribe();
      itrNirfSubscription?.unsubscribe();
    });
  }

  public disabledEnableControl(form: FormGroup): void {
    const controls = [
      'partnerId',
      'catalogId',
      'entryDate',
      'reportDate',
      'crops',
      'isCollectedByClient',
      'transactionId',
      'totalPaid',
      'paymentDate',
    ];

    this._globalService.disabledEnableControlByFlag(
      form,
      'toPostProtocol',
      controls
    );
  }

  public setValidatorsOnChange(form: FormGroup): void {
    const protocolSubscription = form
      .get('toPostProtocol')
      ?.valueChanges.subscribe((value) => {
        if (value) {
          this.setPutValidators(form);
        }
      });

    this._destroyRef.onDestroy(() => {
      protocolSubscription?.unsubscribe();
    });
  }

  public resetPOSTModalForm(): void {
    this.propertyModalForm.set([]);
    this.propertyModalForm.set(this.getPostModalFormWithClient());
  }

  public resetPUTModalForm(): void {
    this.propertyModalForm.set([]);
    this.propertyModalForm.set(this.getPutModalForm());
  }
}
