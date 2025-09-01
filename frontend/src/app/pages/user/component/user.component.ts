import { Component, inject, OnInit, WritableSignal } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { UserService } from '../service/user.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';
import { TableComponent } from '../../../core/shell/table/table.component';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { IUserDetails } from '../../../shared/interfaces/IUserDetails.interface';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './user.component.html',
})
export class UserComponent implements IComponent, OnInit {
  private _userService = inject(UserService);
  private _method: 'POST' | 'PUT' = 'POST';
  private _authenticationService = inject(AuthenticationService);

  public datum: WritableSignal<any[]> = this._userService.entities;
  public header: string[] = this._userService.getHeader();
  public actions: IAction[] = [
    {
      validator: null,
      buttonName: 'Redefinir Senha',
      func: (user: IUserDetails) => {
        const userId = user.details.userId;
        this._userService.resetPassword(userId);
      },
    },
  ];
  public title: string = 'Usuário';
  public newRecord: string = 'Novo Usuário';
  public modalForm: IModalForm[] = this._userService.getPostModalForm();
  public form: FormGroup<any> = this._userService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.users;

  public ngOnInit(): void {
    this._userService.getEntities();

    this._userService.disabledEnableControlByRoleId(this.form);
  }

  onSubmitForm = (submitForm: ISubmitForm): void => {
    const body = this._userService.getRequestBody(submitForm, this._method);

    this._userService.makeEntityUpsertRequest(this._method, body);
  };

  public onSetForm(formData: ISetForm): void {
    const { method, toUpdateData } = formData;

    this._method = method;

    if (method === 'POST') {
      this.modalForm = this._userService.getPostModalForm();
      return;
    }

    this.modalForm = this._userService.getPutModalForm();
    this.form.patchValue(toUpdateData);
  }
}
