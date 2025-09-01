import { Component, inject, input, output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';

import { PropertyNameService } from '../../services/property-name.service';
import { NotificationsService } from '../../services/notifications.service';

import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { IModelFormData } from '../../../shared/interfaces/IModalFormData.interface';
import { AppNotification } from '../../../shared/models/AppNotification.model';
import { NotificationMessage } from '../../../shared/models/NotificationMessage.model';

@Component({
  selector: 'app-password-modal',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatChipsModule,
    MatIconModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
  ],
  templateUrl: './password-modal.component.html',
  styleUrl: './password-modal.component.scss',
})
export class PasswordModalComponent {
  private _propertyNameService = inject(PropertyNameService);
  private _notificationService = inject(NotificationsService);

  public modalTitle = input.required<string>();
  public modalForm = input.required<IModalForm[]>();
  public formGroup = input.required<FormGroup<any>>();
  public formEmitter = output<FormGroup<any>>();
  public input = output<IModelFormData>();
  public isDisabled = false;

  public saveData(): void {
    const errors = this.setError();

    if (errors.length > 0) {
      const notification = new AppNotification(
        new NotificationMessage(
          'Erros de Validação',
          `Erros de validação dos seguintes inputs: ${errors
            .map((error) => this._propertyNameService.getPropertyName(error))
            .join(',')}`
        )
      );
      this._notificationService.openNotification(notification);
      return;
    }
    this.formEmitter.emit(this.formGroup());
  }

  public setError(): string[] {
    const errors: string[] = [];

    if (!this.formGroup().touched) return [];

    if (this.formGroup().errors)
      errors.push('Nova Senha difere de Confirme a Senha');

    for (const [key, control] of Object.entries(this.formGroup().controls)) {
      if (control.errors)
        errors.push(
          `A propriedade ${this._propertyNameService.getPropertyName(
            key
          )} está inválida!`
        );
    }

    return errors;
  }

  public onSetCrops(ids: number[]): void {
    this.formGroup()?.get('crops')?.setValue(ids);
  }

  public toggleIcon(input: IModelFormData):void {
    this.input.emit(input);
  }
}
