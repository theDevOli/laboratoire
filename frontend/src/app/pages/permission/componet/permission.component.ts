import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { PermissionService } from '../service/permission.service';

import { TableComponent } from '../../../core/shell/table/table.component';
import { AuthenticationService } from '../../../core/services/authentication.service';

import { IAction } from '../../../shared/interfaces/IAction.interface';
import { ISetForm } from '../../../shared/interfaces/ISetForm.interface';
import { IComponent } from '../../../shared/interfaces/IComponent.interface';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { ISubmitForm } from '../../../shared/interfaces/ISubmitForm.interface';

@Component({
  selector: 'app-permission',
  standalone: true,
  imports: [TableComponent],
  templateUrl: './permission.component.html',
})
export class PermissionComponent implements IComponent, OnInit {
  private _permissionService = inject(PermissionService);
  private _authenticationService = inject(AuthenticationService);
  private _method!: 'POST' | 'PUT';

  public datum = this._permissionService.entities;
  public header: string[] = this._permissionService.getHeader();
  public actions: IAction[] = [];
  public title: string = 'Nível de Permissão';
  public newRecord: string | null = null;
  public modalForm: IModalForm[] = this._permissionService.getModalForm();
  public form: FormGroup<any> = this._permissionService.getFormGroup();
  public permission: boolean | null | undefined =
    this._authenticationService.auth().permission?.users;

  public ngOnInit(): void {
    this._permissionService.getEntities();
  }

  onSubmitForm = (submitForm: ISubmitForm): void => {
    const body = this._permissionService.getRequestBody(
      this._method,
      submitForm
    );

    this._permissionService.makeEntityUpsertRequest(this._method, body);
  };

  onSetForm(formData: ISetForm): void {
    const { method, toUpdateData } = formData;

    this._method = method;

    this.form.patchValue(toUpdateData);
  }
}
