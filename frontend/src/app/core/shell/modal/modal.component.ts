import {
  Component,
  ElementRef,
  inject,
  input,
  output,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';

import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ChipInputComponent } from '../chip-input/chip-input.component';
import { PropertyNameService } from '../../services/property-name.service';
import { NotificationsService } from '../../services/notifications.service';
import { AppNotification } from '../../../shared/models/AppNotification.model';
import { NotificationMessage } from '../../../shared/models/NotificationMessage.model';
import { GlobalDataService } from '../../services/global-data.service';
import { ProtocolService } from '../../../pages/protocol/service/protocol.service';
import { PropertyService } from '../../../pages/property/service/property.service';

declare var bootstrap: any;
@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatChipsModule,
    MatIconModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    ChipInputComponent,
  ],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss',
})
export class ModalComponent {
  private _propertyNameService = inject(PropertyNameService);
  private _notificationService = inject(NotificationsService);
  private _globalDataService = inject(GlobalDataService);
  private _protocolService = inject(ProtocolService);
  private _propertyService = inject(PropertyService);

  private _render = inject(Renderer2);

  public modalTitle = input.required<string>();
  public modalForm = input.required<IModalForm[]>();
  public formGroup = input.required<FormGroup<any>>();
  public formEmitter = output<FormGroup<any>>();

  @ViewChild('myBootstrapModal') myModalElement!: ElementRef;
  private modalInstance: any;

  ngAfterViewInit(): void {
    if (this.myModalElement && this.myModalElement.nativeElement) {
      // Initialize the Bootstrap modal using its native element
      this.modalInstance = new bootstrap.Modal(
        this.myModalElement.nativeElement
      );

      // Add event listener for when the modal is completely hidden
      // The .bind(this) is crucial to ensure 'this' refers to the component instance inside onModalHidden
      this.myModalElement.nativeElement.addEventListener(
        'hidden.bs.modal',
        this.onModalHidden.bind(this)
      );

      // Optional: If you want to listen for when the modal is shown
      this.myModalElement.nativeElement.addEventListener(
        'shown.bs.modal',
        this.onModalShown.bind(this)
      );
    }
  }

  ngOnDestroy(): void {
    // Clean up the event listener to prevent memory leaks
    if (this.myModalElement && this.myModalElement.nativeElement) {
      this.myModalElement.nativeElement.removeEventListener(
        'hidden.bs.modal',
        this.onModalHidden.bind(this)
      );
      this.myModalElement.nativeElement.removeEventListener(
        'shown.bs.modal',
        this.onModalShown.bind(this)
      );
    }
    // Dispose of the Bootstrap modal instance to free up resources
    if (this.modalInstance) {
      this.modalInstance.dispose();
    }
  }

  onModalHidden(): void {
    this._globalDataService.clearClientOptions();
  }

  onModalShown(): void {
    this.setFirstTab();
  }

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

  public async getUser(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value;

    if (this.modalTitle() === 'Protocolo' && target.id === 'clientTaxId') {
      await this._globalDataService.setClientOptionsByTaxId(value);
      this._protocolService.resetForm();
    }

    if (this.modalTitle() === 'Propriedades' && target.id === 'clientTaxId') {
      await this._globalDataService.setClientOptionsByTaxId(value);
      this._propertyService.resetPOSTModalForm();
    }
  }

  private setFirstTab(): void {
    const firstTabName = `#${this.modalForm().at(0)?.tabId}-tab.nav-link`;
    const firstPaneName = `#${this.modalForm().at(0)?.tabId}.tab-pane`;

    this.modalForm().forEach((modal) => {
      const tabName = `#${modal.tabId}-tab.nav-link`;
      const paneName = `#${modal.tabId}.tab-pane`;
      const tabEl = this._render.selectRootElement(tabName, true);
      const paneEl = this._render.selectRootElement(paneName, true);

      this._render.removeClass(tabEl, 'active');
      this._render.removeClass(paneEl, 'active');
      this._render.removeClass(paneEl, 'show');
    });

    const tabEl = this._render.selectRootElement(firstTabName, true);
    const paneEl = this._render.selectRootElement(firstPaneName, true);
    this._render.addClass(tabEl, 'active');
    this._render.addClass(paneEl, 'active');
    this._render.addClass(paneEl, 'show');
  }
}
