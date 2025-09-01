import { NotificationMessage } from "../models/NotificationMessage.model";
import { ToastType } from "../types/ToastType.type";


export interface IAppNotification {
  notification: NotificationMessage;
  toastType: ToastType;
  showNotification: boolean;
}
