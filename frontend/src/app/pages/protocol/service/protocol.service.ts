import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { HttpService } from '../../../core/services/http.service';
import { LoaderService } from '../../../core/services/loader.service';
import { GlobalService } from '../../../core/services/global.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';

import { Utils } from '../../../shared/Utils/Utils';
import { IResult } from '../../../shared/interfaces/IResult.interface';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IReportPost } from '../../../shared/api-contracts/IReportPost.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { ErrorMessage } from '../../../shared/Utils/ErrorMessage';
import { IProtocolPut } from '../../../shared/api-contracts/IProtocolPut.interface';
import { IParameterGet } from '../../../shared/api-contracts/IParameterGet.interface';
import { IProtocolPost } from '../../../shared/api-contracts/IProtocolPost.interface';
import { SuccessMessage } from '../../../shared/Utils/SuccessMessage';
import { IModelFormData } from '../../../shared/interfaces/IModalFormData.interface';
import { AppNotification } from '../../../shared/models/AppNotification.model';
import { IProtocolDetails } from '../../../shared/interfaces/IProtocolDetails.interface';
import { IDisplayProtocolGet } from '../../../shared/api-contracts/IDisplayProtocolGet.interface';
import { AuthenticationService } from '../../../core/services/authentication.service';

@Injectable({
  providedIn: 'root',
})
export class ProtocolService implements IService {
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _notificationService = inject(NotificationsService);
  private _globalDataService = inject(GlobalDataService);
  private _globalService = inject(GlobalService);
  private _authenticationService = inject(AuthenticationService);
  private _destroyRef = inject(DestroyRef);

  public entities = signal<IProtocolDetails[]>([]);
  public protocolModalForm = signal<IModalForm[]>(this.getPutModalForm());

  public async getEntities(year: number): Promise<void> {
    try {
      this._loaderService.setLoading();

      const user = this._authenticationService.auth().user;
      const partnerId = user?.partnerId;
      const clientId = user?.clientId;

      const url = clientId
        ? `${Constants.DISPLAY_PROTOCOL_END_POINT}/${year}/${clientId}/${false}`
        : partnerId
        ? `${Constants.DISPLAY_PROTOCOL_END_POINT}/${year}/${partnerId}/${true}`
        : `${Constants.DISPLAY_PROTOCOL_END_POINT}/${year}`;

      const response = await this._httpService.makeRequestAsync<
        Array<IDisplayProtocolGet>
      >('GET', url);
      if (!response || !response.data) return;
      const protocols = response.data.map(
        (data): IProtocolDetails => ({
          protocolId: data.protocolId,
          clientName: data.clientName,
          clientTaxId: Utils.taxFormatter(data.clientTaxId),
          fullLocation: `${data.city} - ${data.stateCode}, ${
            data.propertyName
          }${data.area ? ' (' : ''}${data.area ?? ''}${data.area ? ')' : ''}`,
          isPaid:
            data.totalPaid && data.totalPaid >= data.price ? 'Sim' : 'Não',
          partnerName: data.partnerName ?? '-',
          details: {
            totalPaid: data.totalPaid,
            entryDate: Utils.dateFormatter(data.entryDate),
            partnerId: data.partnerId,
            reportId: data.reportId,
            isCollectedByClient: data.isCollectedByClient,
            location: data.city,
            area: data.area,
            ccir: data.ccir,
            itrNirf: data.itrNirf,
            propertyName: data.propertyName,
            paymentDate: data.paymentDate ? data.paymentDate : '',
            price: data.price,
            reportDate: data.reportDate,
            results: data.results,
            catalogId: data.catalogId,
            clientId: data.clientId,
            propertyId: data.propertyId,
            city: data.city,
            postalCode: data.postalCode,
            stateId: data.stateId,
            stateCode: data.stateCode,
            crops: data.crops,
            cashFlowId: data.cashFlowId,
            transactionId: data.transactionId,
          },
        })
      );
      this.entities.set(protocols);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getHeader(): string[] {
    return [
      'Protocolo',
      'Nome do Cliente',
      'CPF/CNPJ',
      'Endereco',
      'Pago?',
      // 'Data de Entrada',
      'Parceiro',
      // 'Tipo',
    ];
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      protocolId: new FormControl(''),
      toUpdateValue: new FormControl(false),
      entryDate: new FormControl(''),
      reportDate: new FormControl(''),
      isCollectedByClient: new FormControl(false),
      catalogId: new FormControl(''),
      transactionId: new FormControl(''),
      price: new FormControl(''),
      totalPaid: new FormControl(''),
      paymentDate: new FormControl(''),
      clientTaxId: new FormControl(''),
      clientName: new FormControl(''),
      clientId: new FormControl(''),
      propertyId: new FormControl(''),
      partnerId: new FormControl(''),
      crops: new FormControl([]),
    });
  }

  public setReactiveForm(parameters: IParameterGet[]): FormGroup<any> {
    const reactiveForm = this.getFormGroup();

    for (const data of parameters) {
      const inputQuantity = data.inputQuantity;
      if (inputQuantity === 0) continue;
      reactiveForm.addControl(data.parameterId + 'ValueA', new FormControl(''));

      if (inputQuantity === 2)
        reactiveForm.addControl(
          data.parameterId + 'ValueB',
          new FormControl('')
        );

      if (inputQuantity === 3) {
        reactiveForm.addControl(
          data.parameterId + 'ValueB',
          new FormControl('')
        );
        reactiveForm.addControl(
          data.parameterId + 'ValueC',
          new FormControl('')
        );
      }
    }

    return reactiveForm;
  }

  public getPutModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Protocolo',
        tabId: 'protocol',
        data: [
          {
            type: 'text',
            nameId: 'protocolId',
            label: 'Protocolo',
          },
          {
            type: 'date',

            nameId: 'entryDate',
            label: 'Data de Entrada',
          },
          {
            type: 'date',

            nameId: 'reportDate',
            label: 'Data do Relatório',
          },
          {
            type: 'chips',
            nameId: 'crops',
            label: 'Culturas',
            placeholder: 'Digite uma cultura...',
            options: this._globalDataService.cropOptions(),
          },
          {
            type: 'checkbox',

            nameId: 'isCollectedByClient',
            label: 'Coleta Feita pelo Cliente?',
          },
        ],
      },
      {
        tabName: 'Laudo',
        tabId: 'report',
        data: [
          {
            type: 'checkbox',
            nameId: 'toUpdateValue',
            label: 'Atualizar Valor?',
          },
        ],
      },
      {
        tabName: 'Catálogo',
        tabId: 'catalog',
        data: [
          {
            type: 'dropdown',
            nameId: 'catalogId',
            label: 'Tipo de laudo',
            options: this._globalDataService.catalogOptions(),
          },
          {
            type: 'number',
            nameId: 'price',
            label: 'Preço',
          },
        ],
      },
      {
        tabName: 'Caixa',
        tabId: 'cashFlow',
        data: [
          {
            type: 'dropdown',
            nameId: 'transactionId',
            label: 'Tipo de Transação',
            options: this._globalDataService.transactionOptions(),
          },
          {
            type: 'number',

            nameId: 'totalPaid',
            label: 'Pagamento Total',
            placeholder: '80',
          },
          {
            type: 'date',
            nameId: 'paymentDate',
            label: 'Dia do pagamento',
          },
        ],
      },
      {
        tabName: 'Cliente',
        tabId: 'client',
        data: [
          {
            type: 'text',
            nameId: 'clientTaxId',
            placeholder: '000.000.000-00',
            label: 'CPF/CNPJ',
          },
          {
            type: 'text',
            nameId: 'clientName',
            placeholder: 'Paulo Cesar de Oliveira',
            label: 'Nome',
          },
          {
            type: 'dropdown',
            nameId: 'clientId',
            label: 'Cliente',
            options: this._globalDataService.clientOptions(),
          },
        ],
      },
      {
        tabName: 'Propiedade',
        tabId: 'property',
        data: [
          {
            type: 'radio',
            nameId: 'propertyId',
            label: 'Propriedades',
            options: this._globalDataService.propertyOptions(),
          },
        ],
      },
      {
        tabName: 'Parceiro',
        tabId: 'partner',
        data: [
          {
            type: 'dropdown',
            nameId: 'partnerId',
            label: 'Parceiro',
            options: this._globalDataService.partnerOptions(),
          },
        ],
      },
    ];
  }

  public getPostModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Novo Protocolo',
        tabId: 'protocol',
        data: [
          {
            type: 'checkbox',

            nameId: 'toPostProtocol',
            label: 'Adicionar Protocolo?',
          },
          {
            type: 'dropdown',

            nameId: 'partnerId',
            label: 'Parceiro',
            options: this._globalDataService.partnerOptions(),
          },
          {
            type: 'dropdown',

            nameId: 'catalogId',
            label: 'Tipo de Laudo',
            options: this._globalDataService.catalogOptions(),
          },
          {
            type: 'date',

            nameId: 'entryDate',
            label: 'Data de Entrada',
          },
          {
            type: 'date',

            nameId: 'reportDate',
            label: 'Data do Laudo',
          },
          {
            type: 'chips',
            nameId: 'crops',
            label: 'Culturas',
            placeholder: 'Digite uma cultura...',
            options: this._globalDataService.cropOptions(),
          },
          {
            type: 'dropdown',
            nameId: 'transactionId',
            label: 'Tipo de Transação',
            options: this._globalDataService.transactionOptions(),
          },
          {
            type: 'number',
            nameId: 'totalPaid',
            label: 'Total Pago',
            placeholder: '80',
          },
          {
            type: 'date',

            nameId: 'paymentDate',
            label: 'Data do Pagamento',
          },
          {
            type: 'checkbox',

            nameId: 'isCollectedByClient',
            label: 'Coletado pelo Cliente?',
          },
        ],
      },
    ];
  }

  public setReportParametersInModalForm(
    parameters: IParameterGet[]
  ): IModalForm[] {
    const modalForm = this.getPutModalForm();

    const data = parameters
      .filter((d) => d.inputQuantity > 0)
      .map((d): IModelFormData => {
        return {
          type: 'number',
          placeholder: '0',
          nameId: d.parameterId + 'ValueA',
          min: 0,
          nameIdB: d.inputQuantity >= 2 ? d.parameterId + 'ValueB' : undefined,
          nameIdC: d.inputQuantity === 3 ? d.parameterId + 'ValueC' : undefined,
          label: d.parameterName,
        };
      });

    return modalForm.map((form): IModalForm => {
      if (form.tabId.toLowerCase() === 'report')
        return {
          tabId: form.tabId,
          tabName: form.tabName,
          data: [...form.data, ...data],
        };
      return form;
    });
  }

  public async getReport(data: IProtocolDetails): Promise<void> {
    const user = this._authenticationService.auth().user;
    const isPartner = user?.partnerId === null ? false : true;
    const isClient = user?.clientId === null ? false : true;
    const reportId = data.details.reportId;
    const protocolId = data.protocolId;

    if (!reportId && (isClient || isPartner)) {
      const notification = new AppNotification(ErrorMessage.waitReport);
      this._notificationService.openNotification(notification);
      return;
    }

    if (isClient && !data.details.cashFlowId) {
      const notification = new AppNotification(ErrorMessage.noPaidReport);
      this._notificationService.openNotification(notification);
      return;
    }

    if (!reportId) {
      const notification = new AppNotification(ErrorMessage.noReport);
      this._notificationService.openNotification(notification);
      return;
    }

    await this._httpService.getDocumentAsync(
      Constants.GENERATE_REPORT_END_POINT + reportId,
      protocolId
    );
  }

  public async reportRequest(data: IReportPost): Promise<void> {
    try {
      const reportId = this.entities().find(
        (protocol) => protocol.protocolId === data.protocolId
      )?.details?.reportId;

      const isPost = reportId === null;

      const body = isPost ? data : { ...data, reportId };

      await this._httpService.makeRequestAsync(
        isPost ? 'POST' : 'PATCH',
        isPost
          ? Constants.REPORT_END_POINT
          : Constants.REPORT_END_POINT + `/${reportId}`,
        body
      );
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    }
  }

  public getProtocolPostBody(
    form: FormGroup,
    clientId: number,
    propertyId: number
  ): IProtocolPost {
    const now = new Date(Date.now());
    const after3Days = new Date(now.setDate(now.getDate() + 3));

    const totalPaid = Number(form.get('totalPaid')?.value) || null;
    const paymentDate = form.get('paymentDate')?.value || null;

    const protocol: IProtocolPost = {
      clientId: clientId,
      propertyId: propertyId,
      partnerId: form.get('partnerId')?.value || null,
      catalogId: Number(form.get('catalogId')?.value),
      entryDate:
        form.get('entryDate')?.value || now.toISOString().split('T')[0],
      reportDate:
        form.get('reportDate')?.value || after3Days.toISOString().split('T')[0],
      crops: form.get('crops')?.value || null,
      isCollectedByClient: form.get('isCollectedByClient')?.value || false,
      transactionId: Number(form.get('transactionId')?.value),
      totalPaid,
      paymentDate:
        totalPaid && !paymentDate
          ? Utils.dateFormatter(new Date(), true)
          : paymentDate,
    };

    return protocol;
  }

  public getRequestBody(submitForm: ISubmitForm): IProtocolPut {
    const { form, data } = submitForm;

    const parameters = this._globalDataService
      .parameter()
      .filter((p) => p.catalogId === data?.details?.catalogId);

    let patchReport: IResult[] = [];
    const toUpdateValue = form.get('toUpdateValue')?.value;
    const crops = form.get('crops')?.value;
    const totalPaid = Number(form.get('totalPaid')?.value) || null;
    const paymentDate =
      form.get('paymentDate')?.value === 'NaN-NaN-NaN'
        ? null
        : form.get('paymentDate')?.value;
    const partnerId = form.get('partnerId')?.value;
    const tempResults = data?.details.results;

    if (toUpdateValue)
      for (const key of Object.keys(form.controls)) {
        if (!key.includes('Value')) continue;

        const control = form.get(key);
        const value = control?.value;
        const parameterId = Number(key.split('Value')[0]);
        const equation = parameters.find(
          (p) => p.parameterId === parameterId
        )?.equation;

        if (key.includes('ValueA') && value !== null && value !== '') {
          const result: IResult = {
            parameterId: parameterId,
            valueA: Number(value),
            equation,
          };
          patchReport.push(result);
        }

        if (key.includes('ValueB') && value !== null && value !== '') {
          patchReport = patchReport.map((res) => {
            if (res.parameterId === parameterId)
              return {
                parameterId: parameterId,
                valueA: res.valueA,
                valueB: Number(value),
                equation,
              };
            return res;
          });
        }

        if (key.includes('ValueC') && value !== null && value !== '') {
          patchReport = patchReport.map((res) => {
            if (res.parameterId === parameterId)
              return {
                parameterId: parameterId,
                valueA: res.valueA,
                valueB: res.valueB,
                valueC: Number(value),
                equation,
              };
            return res;
          });
        }
      }

    return {
      protocolId: form.get('protocolId')?.value,
      reportId: data.details.reportId,
      transactionId: Number(form.get('transactionId')?.value),
      cashFlowId: data.details.cashFlowId,
      results: toUpdateValue
        ? patchReport
        : tempResults?.length > 0 || tempResults !== null
        ? tempResults
        : null,
      catalogId: form.get('catalogId')?.value,
      clientId: form.get('clientId')?.value,
      crops: crops?.length > 0 ? crops : null,
      entryDate: form.get('entryDate')?.value,
      isCollectedByClient: form.get('isCollectedByClient')?.value,
      partnerId: partnerId?.includes('*') ? null : partnerId,
      propertyId: form.get('propertyId')?.value,
      reportDate: form.get('reportDate')?.value,
      totalPaid,
      paymentDate: paymentDate
        ? paymentDate
        : Utils.dateFormatter(new Date(), true),
    };
  }

  public async makeEntityInsertRequest(data: any): Promise<void> {
    try {
      this._loaderService.setLoading();
      await this._httpService.makeRequestAsync(
        'POST',
        Constants.PROTOCOL_END_POINT,
        data
      );
      this._notificationService.openNotification(
        new AppNotification(SuccessMessage.protocolAdded, 'success')
      );
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public async makeEntityUpdateRequest(
    data: IProtocolPut,
    year: number
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const response = await this._httpService.makeRequestAsync(
        'PUT',
        `${Constants.PROTOCOL_END_POINT}/${data.protocolId}`,
        data
      );

      if (!response?.error) this.getEntities(year);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public getReportValues(data: any, toUpdateData: any): any {
    (data?.details?.results as IResult[])?.forEach((el) => {
      const key = `${el.parameterId}`;
      if (el.valueC)
        toUpdateData = { ...toUpdateData, [key + 'ValueC']: el.valueC };

      if (el.valueB)
        toUpdateData = { ...toUpdateData, [key + 'ValueB']: el.valueB };

      toUpdateData = { ...toUpdateData, [key + 'ValueA']: el.valueA };
    });

    return toUpdateData;
  }

  public disabledEnableControl(
    form: FormGroup,
    resultControlsNames: string[] | undefined
  ): void {
    const controls = ['protocolId', 'price', 'clientName'];

    controls.forEach((control) => form.get(control)?.disable());

    if (!resultControlsNames) return;
    resultControlsNames.forEach((control) => form.get(control)?.disable());

    const subscription = form
      .get('toUpdateValue')
      ?.valueChanges.subscribe((value) => {
        if (!value) {
          resultControlsNames.forEach((control) =>
            form.get(control)?.disable()
          );
          return;
        }
        resultControlsNames.forEach((control) => form.get(control)?.enable());
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }

  public async setPropertiesOnChange(form: FormGroup): Promise<void> {
    const subscription = form
      .get('clientId')
      ?.valueChanges.subscribe((value) => {
        if (value) this._globalDataService.getPropertyOptions(value);
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }

  public setValidatorsOnChange(
    form: FormGroup,
    controls: string[] | undefined
  ): void {
    if (!controls) return;
    const subscription = form
      .get('toUpdateValue')
      ?.valueChanges.subscribe((value) => {
        if (value) {
          this._globalService.setValidator(form, controls);
          return;
        }
        this._globalService.removeValidator(form, controls);
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }

  public getDate(): {
    entryDate: string;
    reportDate: string;
  } {
    const now = new Date(Date.now());
    const reportDate = new Date(now);
    reportDate.setDate(reportDate.getDate() + 3);

    if (reportDate.getDay() === 6) {
      reportDate.setDate(reportDate.getDate() + 2);
    }
    if (reportDate.getDay() === 0) {
      reportDate.setDate(reportDate.getDate() + 1);
    }

    return {
      entryDate: Utils.dateFormatter(now, true),
      reportDate: Utils.dateFormatter(reportDate, true),
    };
  }

  public async resetForm() {
    this.protocolModalForm.set([]);
    this.protocolModalForm.set(this.getPutModalForm());
  }
}
