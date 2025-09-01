import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Utils } from '../../../shared/Utils/Utils';
import { IService } from '../../../shared/interfaces/IService.interface';
import { Constants } from '../../../shared/Utils/Constants';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { HttpService } from '../../../core/services/http.service';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { ICashFlowPut } from '../../../shared/api-contracts/ICashFlowPut.interface';
import { ICashFlowPost } from '../../../shared/api-contracts/ICashFlowPost.interface';
import { ICashFlowDetails } from '../../../shared/interfaces/ICashFlowDetails.interface';

import { LoaderService } from '../../../core/services/loader.service';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { NotificationsService } from '../../../core/services/notifications.service';
import { IProtocolPatchCashFlow } from '../../../shared/api-contracts/IProtocolPatchCashFlow.interface';
import { AppNotification } from '../../../shared/models/AppNotification.model';
import { SuccessMessage } from '../../../shared/Utils/SuccessMessage';
import { IDisplayCashFlow } from '../../../shared/api-contracts/IDisplayCashFlow.interface';
import { AuthenticationService } from '../../../core/services/authentication.service';

@Injectable({
  providedIn: 'root',
})
export class CashService implements IService {
  private _httpService = inject(HttpService);
  private _notificationService = inject(NotificationsService);
  private _loaderService = inject(LoaderService);
  private _globalDataService = inject(GlobalDataService);
  private _authenticationService = inject(AuthenticationService);
  private _destroyRef = inject(DestroyRef);

  public entities = signal<ICashFlowDetails[]>([]);

  public async getEntities(
    year: number,
    month: number,
    cashFlowFilter: string,
    transactionFilter: number
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const partnerId = this._authenticationService.auth().user?.partnerId;

      let params: Record<string, string | number | boolean> = {
        cashFlowFilter,
        transactionFilter,
      };
      if (partnerId) params = { ...params, partnerId };
      const response =
        await this._httpService.makeRequestAsync<IDisplayCashFlow>(
          'GET',
          `${Constants.CASH_FLOW_END_POINT}/${year}/${month}`,
          null,
          params
        );
      if (!response) return;
      const { cashFlow, totalAmount } = response.data;
      this._globalDataService.setTotalAmount(totalAmount);
      const tempCashFlow = cashFlow.map(
        (cash): ICashFlowDetails => ({
          description: cash.description,
          totalPaid: Utils.toCash(cash.totalPaid),
          paymentDate: cash.paymentDate
            ? Utils.dateFormatter(cash.paymentDate)
            : '',
          details: {
            cashFlowId: cash.cashFlowId,
            totalPaid: cash.totalPaid,
            transactionId: cash.transactionId,
          },
        })
      );
      this.entities.set(tempCashFlow);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  getModalForm(): IModalForm[] {
    return [
      {
        tabName: 'Caixa',
        tabId: 'cashFlow',
        data: [
          {
            type: 'text',
            nameId: 'description',
            label: 'Descrição',
            placeholder: 'Entrada de: 70, referente ao protocolo 0001/2025',
          },
          {
            type: 'number',
            nameId: 'totalPaid',
            label: 'Total Pago',
            placeholder: '70',
          },
          {
            type: 'date',
            nameId: 'paymentDate',
            label: 'Data do Pagamento',
          },
          {
            type: 'dropdown',
            nameId: 'transactionId',
            label: 'Tipo de Transação',
            options: this._globalDataService.transactionOptions(),
          },
        ],
      },
    ];
  }

  public getPostModalForm(): IModalForm[] {
    return [...this.getModalForm()];
  }

  public getPutModalForm(): IModalForm[] {
    const putModalForm = [...this.getModalForm()];
    const year = new Date().getFullYear();

    putModalForm.push({
      tabName: 'Vincular Protocolo',
      tabId: 'addProtocol',
      data: [
        {
          type: 'checkbox',
          nameId: 'toAddProtocol',
          label: 'Adicionar Protocolo?',
        },
        {
          type: 'text',
          nameId: 'protocolId',
          label: 'Protocolo',
          placeholder: `15/${year}`,
        },
      ],
    });

    return putModalForm;
  }

  public getFormGroup(): FormGroup<any> {
    return new FormGroup({
      description: new FormControl('', Validators.required),
      transactionId: new FormControl('', Validators.required),
      totalPaid: new FormControl(0, Validators.required),
      paymentDate: new FormControl(''),
      toAddProtocol: new FormControl(false),
      protocolId: new FormControl(''),
    });
  }

  public getHeader(): string[] {
    return ['Descrição', 'Total Pago', 'Data do Pagamento'];
  }

  public getRequestBody(
    method: 'POST' | 'PUT',
    submitForm: ISubmitForm
  ): ICashFlowPost | ICashFlowPut {
    const { data, form } = submitForm;
    const details = data as ICashFlowDetails;

    const description = form.get('description')?.value;
    const transactionId = Number(form.get('transactionId')?.value);
    const totalPaid = Number(form.get('totalPaid')?.value);
    const tempPaymentDate = form.get('paymentDate')?.value || null;
    const paymentDate = !tempPaymentDate
      ? Utils.dateFormatter(new Date(), true)
      : tempPaymentDate;

    if (method === 'POST')
      return {
        description,
        transactionId,
        totalPaid,
        paymentDate,
      };

    const cashFlowId = details.details.cashFlowId;
    return {
      cashFlowId,
      description,
      transactionId,
      totalPaid,
      paymentDate,
    };
  }

  public getProtocolBodyRequest(
    submitForm: ISubmitForm
  ): IProtocolPatchCashFlow {
    const { form, data } = submitForm;

    const tempProtocolId = form.get('protocolId')?.value;
    const year = new Date().getFullYear();

    const protocolId = tempProtocolId.includes('/')
      ? tempProtocolId.padStart(9, '0')
      : `${tempProtocolId}/${year}`.padStart(9, '0');
    const cashFlowId = data.details.cashFlowId;

    return {
      protocolId,
      cashFlowId,
      description: form.get('description')?.value,
    };
  }

  public async makeEntityUpsertRequest(
    method: 'PUT' | 'POST',
    data: any
  ): Promise<void> {
    try {
      this._loaderService.setLoading();

      const year = this._globalDataService.year();
      const month = this._globalDataService.month() + 1;
      const cashFlowFilter = this._globalDataService.cashFlowFilter();
      const transactionFilter = this._globalDataService.transactionFilter();

      const url =
        method === 'POST'
          ? Constants.CASH_FLOW_END_POINT
          : `${Constants.CASH_FLOW_END_POINT}/${data.cashFlowId}`;
      const response = await this._httpService.makeRequestAsync(
        method,
        url,
        data
      );

      if (response && !response.error)
        this.getEntities(year, month, cashFlowFilter, transactionFilter);
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public async makeProtocolRequest(
    body: IProtocolPatchCashFlow
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const year = this._globalDataService.year();
      const month = this._globalDataService.month() + 1;
      const cashFlowFilter = this._globalDataService.cashFlowFilter();
      const transactionFilter = this._globalDataService.transactionFilter();
      const response = await this._httpService.makeRequestAsync(
        'PATCH',
        `${Constants.PROTOCOL_END_POINT}/CashFlow/${body.cashFlowId}`,
        body
      );
      if (!response?.error)
        this.getEntities(year, month, cashFlowFilter, transactionFilter);
      this._notificationService.openNotification(
        new AppNotification(SuccessMessage.protocolUpdated, 'success')
      );
    } catch (error) {
      this._notificationService.setFetchErrorNotification();
    } finally {
      this._loaderService.setLoading();
    }
  }

  public setValidatorOnChange(form: FormGroup): void {
    const subscription = form
      .get('toAddProtocol')
      ?.valueChanges.subscribe((value) => {
        if (!value) {
          form.get('protocolId')?.addValidators(Validators.required);
          form.get('protocolId')?.updateValueAndValidity();
          return;
        }
        form.get('protocolId')?.removeValidators(Validators.required);
        form.get('protocolId')?.updateValueAndValidity();
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }

  public disableEnableControls(form: FormGroup): void {
    const subscription = form
      .get('toAddProtocol')
      ?.valueChanges.subscribe((value) => {
        if (!value) {
          form.get('protocolId')?.disable();
          return;
        }

        form.get('protocolId')?.enable();
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }
}
