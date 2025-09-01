import {
  Component,
  DestroyRef,
  effect,
  inject,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { toObservable } from '@angular/core/rxjs-interop';

import { TableComponent } from '../../../core/shell/table/table.component';
import { GlobalDataService } from '../../../core/services/global-data.service';
import { AuthenticationService } from '../../../core/services/authentication.service';

import { ProtocolService } from '../service/protocol.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { IProtocolDetails } from '../../../shared/interfaces/IProtocolDetails.interface';

@Component({
  selector: 'app-protocol',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './protocol.component.html',
})
export class ProtocolComponent implements IComponent, OnInit {
  private _destroyRef = inject(DestroyRef);
  private _protocolService = inject(ProtocolService);
  private _globalDataService = inject(GlobalDataService);
  private _authenticationService = inject(AuthenticationService);
  private _year = this._globalDataService.year;

  public datum = this._protocolService.entities;
  public header = this._protocolService.getHeader();
  public id = 'protocolId';
  public title = 'Protocolo';
  public actions: IAction[] = [
    {
      validator: 'results',
      buttonName: 'Relatório',
      func: (data: IProtocolDetails) => {
        this._protocolService.getReport(data).then();
      },
    },
  ];
  public newRecord: string | null = null;
  public form: FormGroup<any> = this._protocolService.getFormGroup();
  public modalForm: IModalForm[] = this._protocolService.protocolModalForm();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.protocol;

  constructor() {
    effect(() => {
      this.modalForm = this._protocolService.protocolModalForm();
    });

    const year$ = toObservable(this._year);
    const subscription = year$.subscribe(async (year) => {
      await this._protocolService.getEntities(year);
    });

    this._destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ngOnInit(): void {
    this._globalDataService.setIsChipDisabled(false);
  }

  onSubmitForm = async (submitForm: ISubmitForm): Promise<void> => {
    const body = this._protocolService.getRequestBody(submitForm);

    await this._protocolService.makeEntityUpdateRequest(body, this._year());
  };

  public async onSetForm(formData: ISetForm): Promise<void> {
    let { data, toUpdateData } = formData;

    const parameters = this._globalDataService
      .parameter()
      .filter((parameter) => parameter.catalogId === toUpdateData.catalogId);

    await this._globalDataService.updateClientAndPropertyOptions(
      data?.details?.clientId,
      data?.clientTaxId
    );
    this.modalForm = this._protocolService.getPutModalForm();

    this.form = this._protocolService.setReactiveForm(parameters);
    this.modalForm =
      this._protocolService.setReportParametersInModalForm(parameters);

    toUpdateData = this._protocolService.getReportValues(data, toUpdateData);

    const resultControlsNames = this.modalForm
      .at(1)
      ?.data.map((d) => {
        if (d.nameIdC && d.nameIdB) return [d.nameId, d.nameIdB, d.nameIdC];
        if (d.nameIdB) return [d.nameId, d.nameIdB];
        return [d.nameId];
      })
      .slice(1)
      .flatMap((res) => res);

    this._globalDataService.setInitChips(toUpdateData.crops);
    this._protocolService.disabledEnableControl(this.form, resultControlsNames);
    this._protocolService.setValidatorsOnChange(this.form, resultControlsNames);

    this.form.patchValue(toUpdateData);
  }
}
