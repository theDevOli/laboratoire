import { Component, DestroyRef, inject, OnInit, output } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  NgForm,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { AuthenticationService } from '../../core/services/authentication.service';

import { ILogin } from '../../shared/interfaces/ILogin.interface';
import { Utils } from '../../shared/Utils/Utils';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private _authenticationService = inject(AuthenticationService);
  private _destroyRef = inject(DestroyRef);

  public isBackground = output<boolean>();
  public form = new FormGroup({
    username: new FormControl('', Validators.required),
    userPassword: new FormControl('', Validators.required),
  });

  ngOnInit(): void {
    const isExpired = this._authenticationService.isTokenExpired();
    if (isExpired) return;

    const token = localStorage.getItem('token');
    if (token) this._authenticationService.autoLogin();

    const subscription = this.form
      .get('username')
      ?.valueChanges.subscribe((value) => {
        if (!value) return;

        const tax = value.replace(/-/g, '').replace(/./g, '');

        if (/^-?\d+(\.\d+)?$/.test(tax)) {
          const formatted = Utils.taxFormatter(value);
          this.form?.get('username')?.setValue(formatted, { emitEvent: false });
          return;
        }

        this.form?.get('username')?.setValue(value, { emitEvent: false });
      });

    this._destroyRef.onDestroy(() => subscription?.unsubscribe());
  }

  public async onLogin(): Promise<void> {
    if (this.form.invalid) return;

    const data: ILogin = {
      username: this.form.get('username')?.value?.replace(/[.-]/g, "")||'',
      userPassword: this.form.get('userPassword')?.value||'',
    };

    await this._authenticationService.login(data);

    this.form.reset();
  }
}
