import { Component, effect } from '@angular/core';

import { NotificationsService } from '../../services/notifications.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss',
})
export class ToastComponent {
  public toast = this._notificationService.toastNotifications;

  constructor(private _notificationService: NotificationsService) {
    effect(() => {
      this.toast = _notificationService.toastNotifications;
    });
  }

  closeToast(): void {
    this._notificationService.closeNotification();
  }
}
