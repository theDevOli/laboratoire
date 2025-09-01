import { DestroyRef, inject, Injectable } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';

import { HttpService } from './http.service';
import { GlobalDataService } from './global-data.service';
import { NotificationsService } from './notifications.service';

import { Constants } from '../../shared/Utils/Constants';
import { SuccessMessage } from '../../shared/Utils/SuccessMessage';
import { AppNotification } from '../../shared/models/AppNotification.model';

@Injectable({
  providedIn: 'root',
})
export class GlobalService {
  private _globalDataService = inject(GlobalDataService);
  private _destroyRef = inject(DestroyRef);
  private _httpService = inject(HttpService);
  private _notificationService = inject(NotificationsService);

  private toggleVisibility(
    form: FormGroup,
    flag: string,
    controls: string[]
  ): void {
    const toPost = form.get(flag)?.value;

    for (const control of controls) {
      if (control === 'crops' && toPost) {
        this._globalDataService.setIsChipDisabled(false);
        continue;
      }

      if (control === 'crops' && !toPost) {
        this._globalDataService.setIsChipDisabled(true);
        continue;
      }

      if (toPost) {
        form.get(control)?.enable();
        continue;
      }
      form.get(control)?.disable();
    }
  }

  public setValidatorsWithChange(
    form: FormGroup,
    flag: string,
    setPostValidators: Function
  ): void {
    const subscription = form.get(flag)?.valueChanges.subscribe(() => {
      setPostValidators(form);
    });

    this._destroyRef.onDestroy(() => {
      subscription?.unsubscribe();
    });
  }

  public disabledEnableControlByFlag(
    form: FormGroup,
    flag: string,
    controls: string[]
  ): void {
    const subscription = form.get(flag)?.valueChanges.subscribe(() => {
      this.toggleVisibility(form, flag, controls);
    });

    this._destroyRef.onDestroy(() => {
      subscription?.unsubscribe();
    });
  }

  // public disabledEnableControlByCatalogId(form: FormGroup): void {
  //   const catalogIdSubscription = form
  //     .get('catalogId')
  //     ?.valueChanges.subscribe((value) => {
  //       const catalogId = Number(value);
  //       if (catalogId === 0) return;
  //       const isDisable = catalogId <= 3;
  //       this._globalDataService.setIsChipDisabled(isDisable);
  //     });

  //   this._destroyRef.onDestroy(() => {
  //     catalogIdSubscription?.unsubscribe();
  //   });
  // }

  public setValidator(form: FormGroup, controls: string[]): void {
    controls.forEach((control) => {
      form.get(control)?.setValidators(Validators.required);
      form.get(control)?.updateValueAndValidity();
    });
  }

  public removeValidator(form: FormGroup, controls: string[]): void {
    controls.forEach((control) => {
      form.get(control)?.removeValidators(Validators.required);
      form.get(control)?.updateValueAndValidity();
    });
  }

  public async postProtocolRequest(quantity: number): Promise<boolean> {
    try {
      await this._httpService.makeRequestAsync(
        'POST',
        `${Constants.PROTOCOL_END_POINT}/SpotSaver`,
        quantity
      );
      const notification = new AppNotification(
        SuccessMessage.protocolSpotSaver,
        'success'
      );

      this._notificationService.openNotification(notification);
      return true;
    } catch {
      this._notificationService.setFetchErrorNotification();
      return false;
    }
  }
}
