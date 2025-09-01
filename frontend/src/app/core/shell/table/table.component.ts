import {
  Component,
  effect,
  ElementRef,
  inject,
  input,
  model,
  OnChanges,
  output,
  Renderer2,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ModalComponent } from '../modal/modal.component';
import { GlobalDataService } from '../../services/global-data.service';
import { PaginationComponent } from '../pagination/pagination.component';

import { Utils } from '../../../shared/Utils/Utils';
import { Month } from '../../../shared/types/Month.type';
import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IPagination } from '../../../shared/interfaces/IPagination.interface';
import { GlobalService } from '../../services/global.service';
import { ProtocolService } from '../../../pages/protocol/service/protocol.service';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [PaginationComponent, ModalComponent],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss',
})
export class TableComponent implements OnChanges {
  private _currentData: any;
  private _globalDataService = inject(GlobalDataService);
  private _globalService = inject(GlobalService);
  private _protocolService = inject(ProtocolService);
  // private _render = inject(Renderer2);
  private _letter = '';
  @ViewChild('quantity') quantity!: ElementRef;

  public onSetFom = output<ISetForm>();
  public month = this._globalDataService.month;
  public transactions = [
    {
      nameId: 'all',
      label: 'Todas as Transações',
      value: 0,
    },
    ...this._globalDataService.transactionOptions(),
  ].filter((option) => option.value !== 1);
  public totalAmount = Utils.toCash(this._globalDataService.totalAmount());

  public header = input.required<string[]>();
  public datum = model.required<any[]>();
  public actions = input.required<IAction[]>();
  public title = input.required<string>();
  public newRecord = input.required<string | null>();
  public modalForm = input.required<IModalForm[]>();
  public form = input.required<FormGroup<any>>();
  public onSubmitForm = input.required<Function>();
  public permission = input.required<boolean | null | undefined>();
  public filteredDatum: any[] = [];
  public paginatedDatum: any[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    this.initialize();
  }

  constructor() {
    effect(() => {
      const totalAmount = this._globalDataService.totalAmount();
      this.totalAmount = Utils.toCash(totalAmount);
    });
  }

  public toArray(obj: object): any[] {
    return Object.values(obj);
  }

  public isObject(data: any): boolean {
    return typeof data !== 'object';
  }

  public onFilterSearch(event: Event) {
    const input = event.target as HTMLInputElement;
    this._letter = input.value;
    const tempDatum = this.datum().slice();

    if (this._letter === '') {
      this.filteredDatum = tempDatum;
      return;
    }

    this.filteredDatum = tempDatum.filter((el) => {
      const values: string[] = Object.values(el);
      const lowerCasedValues = values.map((val) =>
        String(val).toLocaleLowerCase()
      );
      return lowerCasedValues.join().includes(this._letter.toLocaleLowerCase());
    });
    if (this.filteredDatum.length === 0) this.paginatedDatum.length = 0;
  }

  public onAddData(): void {
    this.form().reset();
    this.onSetFom.emit({ method: 'POST', data: null, toUpdateData: null });
  }

  public onSetMonth(event: Event): void {
    const span = event.target as HTMLSpanElement;
    if (!span.className.includes('btn')) return;
    const month = span.innerText as Month;
    this._globalDataService.setMonth(month);
  }

  public async onEditData(data: object): Promise<void> {
    this.form().reset();

    let updatedData: any = {};

    this._currentData = data;

    for (const [key, value] of Object.entries(data)) {
      if (typeof value === 'object' && key !== 'results') {
        for (const [key, valueDetail] of Object.entries(value)) {
          const newValue: any = this.getSanitizedValue(key, valueDetail);
          updatedData = { ...updatedData, ...newValue };
        }
        continue;
      }

      const newValue: any = this.getSanitizedValue(key, value);
      updatedData = { ...updatedData, ...newValue };
    }

    this.onSetFom.emit({
      method: 'PUT',
      data,
      toUpdateData: updatedData,
    });
  }

  private getSanitizedValue(
    key: string,
    value: any
  ): {
    [x: string]: any;
  } {
    let newValue: any = value;

    if (
      typeof value === 'string' &&
      (value.toUpperCase() === 'SIM' || value.toUpperCase() === 'NÃO')
    )
      newValue = value.toUpperCase() === 'SIM' ? true : false;

    if (typeof value === 'string' && value.toUpperCase() === '-') newValue = '';

    // if (
    //   typeof value === 'string' &&
    //   value.includes('R$') &&
    //   !value.includes('protocol')
    // )
    //   newValue = Number(value.replace('R$', '').replace(',', '.'));

    if (key.toUpperCase().includes('DATE') || key.toUpperCase().includes('DAY'))
      newValue = Utils.dateFormatter(value, true);

    return { [key]: newValue };
  }

  public onUpdatePagination(pagination: IPagination): void {
    const { startIndex, endIndex } = pagination;
    const paginated = this.filteredDatum.slice(startIndex, endIndex);
    this.paginatedDatum = paginated;
  }

  public onModalSave(form: FormGroup<any>): void {
    const onSubmitForm = this.onSubmitForm();
    onSubmitForm({ form, data: this._currentData });
  }

  private initialize(): void {
    const currentPage = 1;
    const dataPerPage = 25;
    const startIndex = (currentPage - 1) * dataPerPage;
    const endIndex = startIndex + dataPerPage;

    const copyDatum = !this._letter ? this.datum().slice() : this.filteredDatum;

    this.filteredDatum = copyDatum;

    this.paginatedDatum = copyDatum.slice(startIndex, endIndex);
  }

  public onChangeCashFlowFilter(event: Event) {
    const value = (event.target as HTMLSelectElement).value;

    this._globalDataService.setCashFlowFilter(value as 'all' | 'in' | 'out');
  }

  public onChangTransactionFilter(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this._globalDataService.setTransactionFilter(Number(value));
  }

  public async addProtocolSpot(): Promise<void> {
    const quantity = this.quantity.nativeElement.value;
    if (!quantity) return;
    const ok = await this._globalService.postProtocolRequest(Number(quantity));
    this.quantity.nativeElement.value = '';
    if (ok) {
      const year = this._globalDataService.year();
      await this._protocolService.getEntities(year);
      this._globalDataService.setIsChipDisabled(false);
    }
  }
}
