import { Injectable, OnDestroy, signal } from '@angular/core';

import { Constants } from '../../shared/Utils/Constants';
import { AppNotification } from '../../shared/models/AppNotification.model';
import { NotificationMessage } from '../../shared/models/NotificationMessage.model';
import { ErrorMessage } from '../../shared/Utils/ErrorMessage';

@Injectable({
  providedIn: 'root',
})
export class NotificationsService implements OnDestroy {
  private toast = signal<AppNotification>(
    new AppNotification(new NotificationMessage('', ''), 'info', false)
  );
  private _timeout: any = null;

  public toastNotifications = this.toast.asReadonly();

  public closeNotification(): void {
    this.toast.update(
      (cur) => new AppNotification(cur.notification, cur.toastType, false)
    );
  }

  public openNotification(appNotification: AppNotification): void {
    const { notification, toastType } = appNotification;

    this.toast.set(new AppNotification(notification, toastType, true));
    
    this._timeout = setTimeout(() => {
      this.closeNotification();
    }, Constants.TOAST_DURATION);
  }

  public setFetchErrorNotification(): void {
    const notification = new AppNotification(ErrorMessage.fetchError);
    this.openNotification(notification);
  }

  ngOnDestroy(): void {
    if (this._timeout !== null) clearTimeout(this._timeout);
  }
}
