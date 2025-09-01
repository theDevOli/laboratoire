import {
  Component,
  effect,
  inject,
  input,
  output,
  ViewChild,
} from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

import { IModelFormData } from '../../../shared/interfaces/IModalFormData.interface';
import { IModalOptions } from '../../../shared/interfaces/IModalOptions.interface';
import { GlobalDataService } from '../../services/global-data.service';

@Component({
  selector: 'app-chip-input',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './chip-input.component.html',
  styleUrl: './chip-input.component.scss',
})
export class ChipInputComponent {
  private _globalDataService = inject(GlobalDataService);

  @ViewChild('inputField') inputField: any;
  public items: Array<{ label: string; value: number }> = [];
  public value = output<number[]>();
  public chipInput = input.required<IModelFormData>();
  public formGroup = input.required<FormGroup<any>>();
  public options: IModalOptions[] = [];
  public removable = false;
  public isDisabled = this._globalDataService.isChipDisabled;

  constructor() {
    effect(() => {
      const isDisable = this.isDisabled();

      if (isDisable) this.onRemoveAll();

      this.items = this._globalDataService.initChips();
    });
  }

  public onChipBarClick(): void {
    this.inputField.nativeElement.focus();
  }

  public onFilteredOption(event: Event): void {
    const options = this.chipInput().options;
    if (!options) return;

    const value = (event.target as HTMLInputElement).value;
    if (value === '') {
      this.options = [];
      return;
    }

    const itemValues = this.items.map((item) => item.value);

    this.options = options
      .filter((option) => !itemValues.includes(Number(option.value)))
      .filter((option) =>
        option.label.toLowerCase().includes(value.toLowerCase())
      );
  }

  public onOptionClick(option: IModalOptions): void {
    const tempItem = { label: option.label, value: Number(option.value) };
    this.items = [...this.items, tempItem];

    this.setValue();

    this.options = [];

    this.removable = this.items.length > 0;
    this.inputField.nativeElement.value = '';
  }

  public onRemoveItem(index: number): void {
    const tempItems = this.items.filter((_item, i) => i !== index);
    this.items = [...tempItems];

    this.setValue();

    this.removable = this.items.length > 0;
  }

  public onRemoveAll(): void {
    this.items = [];
    this.removable = false;
  }

  private setValue(): void {
    const tempValue = this.items.map((val) => val.value);
    this.value.emit(tempValue);
  }
}
