import { inject, Injectable, signal } from '@angular/core';
import { HttpService } from './http.service';
import { IPartnerGet } from '../../shared/api-contracts/IPartnerGet.interface';
import { Constants } from '../../shared/Utils/Constants';
import { ICatalogGet } from '../../shared/api-contracts/ICatalogGet.interface';
import { ICropGet } from '../../shared/api-contracts/ICropGet.interface';
import { ITransactionGet } from '../../shared/api-contracts/ITransactionGet.interface';
import { IModalOptions } from '../../shared/interfaces/IModalOptions.interface';
import { IProtocolYearGet } from '../../shared/api-contracts/IProtocolYearGet.interface';
import { IStateGet } from '../../shared/api-contracts/IStateGet.interface';
import { IParameterGet } from '../../shared/api-contracts/IParameterGet.interface';
import { IClientPut } from '../../shared/api-contracts/IClientPut.interface';
import { Utils } from '../../shared/Utils/Utils';
import { LoaderService } from './loader.service';
import { IPropertyGet } from '../../shared/api-contracts/IPropertyGet.interface';
import { Month } from '../../shared/types/Month.type';
import { IRoleGet } from '../../shared/api-contracts/IRoleGet.interface';
import { ApiResponse } from '../../shared/models/ApiResponse.model';
import { toObservable } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class GlobalDataService {
  private _loaderService = inject(LoaderService);
  private _httpService = inject(HttpService);

  private _years = signal<number[]>([]);
  // private _year = signal<number>(new Date().getFullYear());
  private _year = signal<number>(new Date().getFullYear());
  private _month = signal<number>(new Date().getMonth());
  private _cashFlowFilter = signal<'all' | 'in' | 'out'>('all');
  private _transactionFilter = signal<number>(0);
  private _totalAmount = signal<number>(0);
  private _isChipDisabled = signal<boolean>(false);
  private _initChips = signal<{ label: string; value: number }[]>([]);

  private _parameter = signal<IParameterGet[]>([]);
  private _partnerOptions = signal<IModalOptions[]>([]);
  private _catalogOptions = signal<IModalOptions[]>([]);
  private _cropOptions = signal<IModalOptions[]>([]);
  private _transactionOptions = signal<IModalOptions[]>([]);
  private _stateOptions = signal<IModalOptions[]>([]);
  private _clientOptions = signal<IModalOptions[]>([]);
  private _propertyOptions = signal<IModalOptions[]>([]);
  private _roleOptions = signal<IModalOptions[]>([]);

  public years = this._years.asReadonly();
  public year = this._year;
  public month = this._month.asReadonly();
  public cashFlowFilter = this._cashFlowFilter.asReadonly();
  public transactionFilter = this._transactionFilter.asReadonly();
  public totalAmount = this._totalAmount.asReadonly();
  public isChipDisabled = this._isChipDisabled.asReadonly();
  public initChips = this._initChips.asReadonly();

  public partnerOptions = this._partnerOptions.asReadonly();
  public catalogOptions = this._catalogOptions.asReadonly();
  public cropOptions = this._cropOptions.asReadonly();
  public transactionOptions = this._transactionOptions.asReadonly();
  public stateOptions = this._stateOptions.asReadonly();
  public parameter = this._parameter.asReadonly();
  public clientOptions = this._clientOptions.asReadonly();
  public propertyOptions = this._propertyOptions.asReadonly();
  public roleOptions = this._roleOptions.asReadonly();

  public setYear(year: number) {
    this._year.set(year);
  }

  public setMonth(month: Month) {
    const months = {
      Jan: 0,
      Fev: 1,
      Mar: 2,
      Abr: 3,
      Mai: 4,
      Jun: 5,
      Jul: 6,
      Ago: 7,
      Set: 8,
      Out: 9,
      Nov: 10,
      Dez: 11,
    };
    this._month.set(months[month]);
  }

  public setCashFlowFilter(cashFlow: 'all' | 'in' | 'out'): void {
    this._cashFlowFilter.set(cashFlow);
  }

  public setTransactionFilter(transaction: number): void {
    this._transactionFilter.set(transaction);
  }

  public setTotalAmount(amount: number): void {
    this._totalAmount.set(amount);
  }

  public setIsChipDisabled(isChipDisabled: boolean) {
    this._isChipDisabled.set(isChipDisabled);
  }

  public setInitChips(values: number[]): void {
    const tempItems = values?.map(
      (value: number): { label: string; value: number } => {
        const option = this._cropOptions().find(
          (option) => option.value === value
        );
        return { label: option!.label, value: value };
      }
    );
    this._initChips.set([...tempItems]);
  }

  public async cacheData(): Promise<void> {
    try {
      const [
        partnerResponse,
        catalogResponse,
        cropsResponse,
        transactionResponse,
        stateResponse,
        parameterResponse,
        yearResponse,
        roleResponse,
      ] = await Promise.all([
        this._httpService.makeRequestAsync<IPartnerGet[]>(
          'GET',
          Constants.PARTNER_END_POINT
        ),
        this._httpService.makeRequestAsync<ICatalogGet[]>(
          'GET',
          Constants.CATALOG_END_POINT
        ),
        this._httpService.makeRequestAsync<ICropGet[]>(
          'GET',
          Constants.CROP_END_POINT
        ),
        this._httpService.makeRequestAsync<ITransactionGet[]>(
          'GET',
          Constants.TRANSACTION_YEARS_END_POINT
        ),
        this._httpService.makeRequestAsync<IStateGet[]>(
          'GET',
          Constants.UTILS_STATES_END_POINT
        ),
        this._httpService.makeRequestAsync<IParameterGet[]>(
          'GET',
          Constants.PARAMETER_END_POINT
        ),
        this._httpService.makeRequestAsync<IProtocolYearGet[]>(
          'GET',
          Constants.PROTOCOL_YEARS_END_POINT
        ),
        this._httpService.makeRequestAsync<IRoleGet[]>(
          'GET',
          Constants.ROLE_END_POINT
        ),
      ]);
      if (
        !partnerResponse ||
        !catalogResponse ||
        !cropsResponse ||
        !transactionResponse ||
        !stateResponse ||
        !parameterResponse ||
        !yearResponse ||
        !roleResponse
      )
        return;

      const tempPartner = partnerResponse.data.map(
        (partner): IModalOptions => ({
          nameId: partner.partnerId,
          label: partner.partnerName,
          value: partner.partnerId,
        })
      );
      const tempCatalog = catalogResponse.data.map(
        (catalog): IModalOptions => ({
          nameId: catalog.catalogId.toString(),
          label: catalog.labelName,
          value: catalog.catalogId,
        })
      );

      const tempCrops = cropsResponse.data.map(
        (crop): IModalOptions => ({
          nameId: crop.cropId.toString(),
          label: crop.cropName,
          value: crop.cropId,
        })
      );

      const tempTransaction = transactionResponse.data.map(
        (transaction): IModalOptions => ({
          nameId: transaction.transactionId.toString(),
          label: `${transaction.transactionType} ${
            transaction.bankName ? ` | ${transaction.bankName}` : ''
          }${transaction.ownerName ? ` | ${transaction.ownerName}` : ''}`,
          value: transaction.transactionId,
        })
      );

      const tempState: IModalOptions[] = stateResponse.data.map((state) => ({
        nameId: state.stateCode.toLocaleUpperCase(),
        label: state.stateName,
        value: state.stateId,
      }));

      const tempYear = yearResponse.data.map((year) => year.year);

      const tempRole = roleResponse.data.map(
        (role): IModalOptions => ({
          nameId: `${role.roleName}-${role.roleId}`,
          label: role.roleName,
          value: role.roleId,
        })
      );

      this._catalogOptions.set([...tempCatalog]);
      this._cropOptions.set([...tempCrops]);
      this._partnerOptions.set([...tempPartner]);
      this._transactionOptions.set([...tempTransaction]);
      this._stateOptions.set([...tempState]);
      this._parameter.set([...parameterResponse.data]);
      this._years.set([...tempYear]);
      this._roleOptions.set([...tempRole]);
    } catch (error) {}
  }

  public async setYearOptions(): Promise<void> {
    try {
      const yearResponse = await this._httpService.makeRequestAsync<
        IProtocolYearGet[]
      >('GET', Constants.PROTOCOL_YEARS_END_POINT);
      if (!yearResponse) return;

      const tempYear = yearResponse.data.map((year) => year.year);
      this._years.set([...tempYear]);
    } catch (error) {}
  }

  public async getClientOptions(): Promise<void> {
    try {
      this._loaderService.setLoading();
      const params = { filter: 'ClientTaxId' };
      const response = await this._httpService.makeRequestAsync<IClientPut[]>(
        'GET',
        Constants.CLIENT_END_POINT,
        null,
        params
      );

      if (!response || !response.data) return;

      this.setClientOptions(response);

      this._loaderService.setLoading();
    } catch (error) {
      this._loaderService.setLoading();
    }
  }

  public async getPropertyOptions(clientId: number): Promise<void> {
    try {
      this._loaderService.setLoading();

      const response = await this._httpService.makeRequestAsync<IPropertyGet[]>(
        'GET',
        Constants.PROPERTY_END_POINT + `/Client/${clientId}`
      );

      if (!response) return;

      this.setPropertyOptions(response);

      this._loaderService.setLoading();
    } catch (error) {
      this._loaderService.setLoading();
    }
  }

  public async updateClientAndPropertyOptions(
    clientId: number,
    taxId: string
  ): Promise<void> {
    try {
      this._loaderService.setLoading();
      const sanitizedTaxId = taxId.replace(/\D/g, '');
      const params = { filter: sanitizedTaxId };
      const [clientRes, propertyRes] = await Promise.all([
        this._httpService.makeRequestAsync<IClientPut[]>(
          'GET',
          Constants.CLIENT_END_POINT + '/Like/TaxId',
          null,
          params
        ),
        this._httpService.makeRequestAsync<IPropertyGet[]>(
          'GET',
          Constants.PROPERTY_END_POINT + `/Client/${clientId}`
        ),
      ]);

      if (!clientRes || !propertyRes) return;

      this.setPropertyOptions(propertyRes);
      this.setClientOptions(clientRes);
    } catch (error) {
      console.error('File: global-data.service.ts', 'Line: 315', error);
    } finally {
      this._loaderService.setLoading();
    }
  }

  private setPropertyOptions(response: ApiResponse<IPropertyGet[]>): void {
    const tempPropertyOptions = response.data.map((property): IModalOptions => {
      const state = this.stateOptions().find(
        (state) => Number(state.value) === property.stateId
      )?.nameId;
      const extraData =
        (property.area ? ` | ${property.area}` : '') +
        (property.ccir
          ? ` | CCIR: ${Utils.ccirFormatter(property.ccir)}`
          : '') +
        (property.itrNirf
          ? ` | ITR/NIRF: ${Utils.itrNirfFormatter(property.itrNirf)}`
          : '') +
        (property.registration ? ` | Matrícula: ${property.registration}` : '');
      return {
        nameId: property.propertyId.toString(),
        label: `${property.city} - ${state}, ${property.propertyName} ${extraData}`,
        value: property.propertyId,
      };
    });

    this._propertyOptions.set([...tempPropertyOptions]);
  }

  private setClientOptions(response: ApiResponse<IClientPut[]>): void {
    const tempClientOptions = response.data.map(
      (client: IClientPut): IModalOptions => ({
        nameId: 'clientId',
        label: `${Utils.taxFormatter(client.clientTaxId)} | ${
          client.clientName
        }`,
        value: client.clientId,
      })
    );

    this._clientOptions.set([...tempClientOptions]);
  }

  public clearClientOptions(): void {
    if (this._clientOptions().length === 0) return;
    this._clientOptions.set([]);
  }

  public async setClientOptionsByTaxId(taxId: string): Promise<void> {
    try {
      this._loaderService.setLoading();
      const sanitizedTaxId = taxId.replace(/\D/g, '');
      const param: Record<string, string> = {
        filter: sanitizedTaxId,
      };
      const clientRes = await this._httpService.makeRequestAsync<IClientPut[]>(
        'GET',
        Constants.CLIENT_END_POINT + '/Like/TaxId',
        null,
        param
      );
      if (!clientRes) return;
      this.setClientOptions(clientRes);
    } catch (error) {
      console.error(error);
    } finally {
      this._loaderService.setLoading();
    }
  }
}
