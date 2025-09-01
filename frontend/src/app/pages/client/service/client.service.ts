import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { NotificationsService } from '../../../core/services/notifications.service';

import { ProtocolService } from '../../protocol/service/protocol.service';
import { PropertyService } from '../../property/service/property.service';

import { Utils } from '../../../shared/Utils/Utils';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IClientPut } from '../../../shared/api-contracts/IClientPut.interface';
import { IClientPost } from '../../../shared/api-contracts/IClientPost.interface';
import { GlobalService } from '../../../core/services/global.service';
import { IClientDetails } from '../../../shared/interfaces/IClientDetails.interface';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { clientTaxIdLengthValidator } from '../../../shared/validator/clientTaxIdLength.validator';

@Injectable({
  providedIn: 'root',
})
export class ClientService implements IService {
  private _httpService = inject(HttpService);
  private _notificationService = inject(NotificationsService);
  private _loaderService = inject(LoaderService);
  private _globalDataService = inject(GlobalDataService);
  private _destroyRef = inject(DestroyRef);
  private _protocolService = inject(ProtocolService);
  private _propertyService = inject(PropertyService);
  private _globalService = inject(GlobalService);

  public entities = signal<IClientDetails[]>([]);

  public async getEntities(): Promise<void> {
    try {
      this._loaderService.setLoading();
      const res = await this._httpService.makeRequestAsync<IClientPut[]>(
        'GET',
        Constants.CLIENT_END_POINT
      );
      if (!res || !res.data) return;
      const displayClients = res.data.map(
        (client: IClientPut): IClientDetails => ({
          clientName: client.clientName,
          clientTaxId: Utils.taxFormatter(client.clientTaxId),
          clientEmail: client.clientEmail ?? '-',
          clientPhone: client.clientPhone
            ? Utils.phoneFormatter(client.clientPhone)
            : '-',
          details: {
            clientId: client.clientId,
          },
        })
      );
      this.entities.set(displayClients);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getPostModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Cliente',
        tabId: 'client',
        data: [
          {
            type: 'text',
            nameId: 'clientName',
            label: 'Nome',
            placeholder: 'João Batista da Luz de Souza',
          },
          {
            type: 'text',
            nameId: 'clientTaxId',
            label: 'CPF/CNPJ',
            placeholder: '000.000.000-00',
          },
          {
            type: 'email',
            nameId: 'clientEmail',
            label: 'Email',
            placeholder: 'exemplo@email.com',
          },
          {
            type: 'text',
            nameId: 'clientPhone',
            label: 'Contato',
            placeholder: '(79) 9 9998-8765',
          },
        ],
      },
    ];
  }

  public getPutModalForm(): IModalForm[] {
    const postClientModalForm = this.getPostModalForm();
    const postPropertyModalForm = this._propertyService.getPostModalForm().map(
      (property): IModalForm => ({
        tabName: 'Nova Propriedade',
        tabId: property.tabId,
        data: [
          {
            type: 'checkbox',
            nameId: 'toPostProperty',
            label: 'Adicionar Nova Propriedade?',
          },
          ...property.data,
        ],
      })
    );

    const postProtocolModalForm = this._protocolService.getPostModalForm().map(
      (protocol): IModalForm => ({
        tabName: protocol.tabName,
        tabId: protocol.tabId,
        data: [
          ...protocol.data,
          {
            type: 'radio',
            nameId: 'propertyId',
            label: 'Propriedades',
            options: this._globalDataService.propertyOptions(),
          },
        ],
      })
    );

    return [
      ...postClientModalForm,
      ...postPropertyModalForm,
      ...postProtocolModalForm,
    ];
  }

  public getFormGroup(): FormGroup<any> {
    const form = new FormGroup({
      //related to client
      clientName: new FormControl(''),
      clientTaxId: new FormControl('', [
        Validators.required,
        clientTaxIdLengthValidator(),
      ]),
      clientEmail: new FormControl('', Validators.email),
      clientPhone: new FormControl('', Validators.minLength(16)),
      propertyId: new FormControl(''),
      //related to property
      toPostProperty: new FormControl(false),
      area: new FormControl(''),
      registration: new FormControl(''),
      city: new FormControl(''),
      postalCode: new FormControl('', Validators.minLength(10)),
      stateId: new FormControl(''),
      propertyName: new FormControl(''),
      ccir: new FormControl(''),
      itrNirf: new FormControl(''),
      //Related to protocol
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
    return form;
  }

  public setPostValidators(form: FormGroup): void {
    const setValidators = ['clientName'];
    this._globalService.setValidator(form, setValidators);
  }

  public setPutValidators(form: FormGroup): void {
    let setValidators = ['clientName'];
    const toPostProperty = form.get('toPostProperty')?.value;
    const toPosProtocol = form.get('toPostProtocol')?.value;

    if (toPostProperty)
      setValidators = [...setValidators, 'city', 'stateId', 'propertyName'];

    if (toPosProtocol)
      setValidators = [
        ...setValidators,
        'propertyId',
        'catalogId',
        'entryDate',
        'transactionId',
      ];

    this._globalService.setValidator(form, setValidators);
  }

  public getHeader(): string[] {
    return ['Nome', 'CPF/CNPJ', 'Email', 'Contato'];
  }

  public async makeEntityUpsertRequest(
    method: 'POST' | 'PUT',
    body: IClientPut | IClientPost
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const url =
        method === 'POST'
          ? Constants.CLIENT_END_POINT
          : Constants.CLIENT_END_POINT + `/${(body as IClientPut).clientId}`;
      const res = await this._httpService.makeRequestAsync(method, url, body);
      if (!res?.error) this.getEntities();
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getUpsertBodyRequest(
    form: FormGroup,
    method: 'PUT' | 'POST',
    clientId: string
  ) {
    const body: IClientPut | IClientPost =
      method === 'PUT'
        ? {
            clientId: clientId,
            clientName: form.get('clientName')?.value,
            clientTaxId: Utils.taxFormatter(
              form.get('clientTaxId')?.value,
              true
            ),
            clientEmail: form.get('clientEmail')?.value || null,
            clientPhone:
              Utils.phoneFormatter(form.get('clientPhone')?.value, true) ||
              null,
          }
        : {
            clientName: form.get('clientName')?.value,
            clientTaxId: Utils.taxFormatter(
              form.get('clientTaxId')?.value,
              true
            ),
            clientEmail: form.get('clientEmail')?.value || null,
            clientPhone:
              Utils.phoneFormatter(form.get('clientPhone')?.value, true) ||
              null,
          };

    return body;
  }

  public async isClientOnDB(taxId: string): Promise<boolean> {
    try {
      const response = await this._httpService.makeRequestAsync(
        'GET',
        Constants.CLIENT_END_POINT + `/TaxId/${taxId}`
      );

      return response !== null && response.data !== null;
    } catch (error) {
      return false;
    }
  }

  public controlFormatter(form: FormGroup): void {
    const phoneSubscription = form
      .get('clientPhone')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.phoneFormatter(value);
        form.get('clientPhone')?.setValue(formatted, { emitEvent: false });
      });

    const taxSubscription = form
      .get('clientTaxId')
      ?.valueChanges.subscribe((value) => {
        const formatted = Utils.taxFormatter(value);
        form.get('clientTaxId')?.setValue(formatted, { emitEvent: false });
      });

    this._destroyRef.onDestroy(() => {
      phoneSubscription?.unsubscribe();
      taxSubscription?.unsubscribe();
    });
  }

  public disabledEnableControl(form: FormGroup): void {
    const protocolControls = [
      'propertyId',
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
    const propertyControls = [
      'area',
      'city',
      'registration',
      'postalCode',
      'stateId',
      'propertyName',
      'ccir',
      'itrNirf',
    ];

    this._globalService.disabledEnableControlByFlag(
      form,
      'toPostProtocol',
      protocolControls
    );
    this._globalService.disabledEnableControlByFlag(
      form,
      'toPostProperty',
      propertyControls
    );

    // this._globalService.disabledEnableControlByCatalogId(form);
  }

  public setValidatorsOnChange(form: FormGroup): void {
    const protocolSubscription = form
      .get('toPostProtocol')
      ?.valueChanges.subscribe((value) => {
        if (value) {
          form.get('toPostProperty')?.setValue(false);
          this.setPutValidators(form);
        }
      });

    const propertySubscription = form
      .get('toPostProperty')
      ?.valueChanges.subscribe((value) => {
        if (value) {
          form.get('toPostProtocol')?.setValue(false);
          this.setPutValidators(form);
        }
      });

    this._destroyRef.onDestroy(() => {
      protocolSubscription?.unsubscribe();
      propertySubscription?.unsubscribe();
    });
  }
}
