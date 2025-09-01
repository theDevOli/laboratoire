import { ToastType } from "../types/ToastType.type";
import { IAppNotification } from "../interfaces/IAppNotification.interface";
import { NotificationMessage } from "./NotificationMessage.model";

export class AppNotification implements IAppNotification {
  private _notification: NotificationMessage;
  private _toastType: ToastType;
  private _showNotification: boolean;

  constructor(
    notification: NotificationMessage,
    toastType: ToastType = 'error',
    showNotification: boolean = true
  ) {
    this._notification = notification;
    this._toastType = toastType;
    this._showNotification = showNotification;
  }

  get notification(): NotificationMessage {
    return this._notification;
  }

  get toastType(): ToastType {
    return this._toastType;
  }

  get showNotification(): boolean {
    return this._showNotification;
  }

  set notification(notification: NotificationMessage) {
    this._notification = notification;
  }

  set toastType(toastType: ToastType) {
    this._toastType = toastType;
  }

  set showNotification(showNotification: boolean) {
    this._showNotification = showNotification;
  }
}
