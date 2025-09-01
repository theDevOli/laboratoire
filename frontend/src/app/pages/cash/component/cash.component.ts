import {
  Component,
  effect,
  inject,
  OnInit,
  WritableSignal,
} from '@angular/core';
import { FormGroup } from '@angular/forms';

import { CashService } from '../service/cash.service';

import { TableComponent } from '../../../core/shell/table/table.component';
import { GlobalDataService } from '../../../core/services/global-data.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { ICashFlowDetails } from '../../../shared/interfaces/ICashFlowDetails.interface';
import { AuthenticationService } from '../../../core/services/authentication.service';

@Component({
  selector: 'app-cash',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './cash.component.html',
  styleUrl: './cash.component.scss',
})
export class CashComponent implements IComponent, OnInit {
  private _cashService = inject(CashService);
  private _globalDataService = inject(GlobalDataService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'PUT' | 'POST';

  public month = this._globalDataService.month;
  public year = this._globalDataService.year;
  public cashFlowFilter = this._globalDataService.cashFlowFilter;
  public transactionFilter = this._globalDataService.transactionFilter;

  public datum: WritableSignal<ICashFlowDetails[]> = this._cashService.entities;
  public header: string[] = this._cashService.getHeader();
  public actions: IAction[] = [];
  public title: string = 'Caixa';
  public newRecord: string | null = 'Nova Movimentação';
  public modalForm: IModalForm[] = this._cashService.getPutModalForm();
  public form: FormGroup<any> = this._cashService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.cashFlow;

  constructor() {
    effect(
      () => {
        const month = this.month() + 1;
        const year = this.year();
        const cashFlowFilter = this.cashFlowFilter();
        const transactionFilter = this.transactionFilter();

        this._cashService.getEntities(
          year,
          month,
          cashFlowFilter,
          transactionFilter
        );
      },
      { allowSignalWrites: true }
    );
  }

  ngOnInit(): void {
    this._cashService.disableEnableControls(this.form);
    this._cashService.setValidatorOnChange(this.form);
  }

  public onSubmitForm = async (submitForm: ISubmitForm): Promise<void> => {
    if (submitForm.form.get('toAddProtocol')?.value) {
      const body = this._cashService.getProtocolBodyRequest(submitForm);
      this._cashService.makeProtocolRequest(body);
      return;
    }

    const body = this._cashService.getRequestBody(this._method, submitForm);

    await this._cashService.makeEntityUpsertRequest(this._method, body);
  };

  public onSetForm(formData: ISetForm): void {
    const { method } = formData;

    this._method = method;

    if (method === 'PUT') {
      this.modalForm = this._cashService.getPutModalForm();
      this.form.patchValue(formData.toUpdateData);
      return;
    }

    this.modalForm = this._cashService.getPostModalForm();
  }
}
