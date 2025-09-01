import { INotificationMessage } from "../interfaces/INotification.interface";

export class NotificationMessage implements INotificationMessage {
  private _title: string;
  private _message: string;

  constructor(title: string, message: string) {
    this._title = title;
    this._message = message;
  }

  get title(): string {
    return this._title;
  }

  get message(): string {
    return this._message;
  }
}
