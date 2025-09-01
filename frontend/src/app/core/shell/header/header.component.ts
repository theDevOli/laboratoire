import { Component, effect, inject, Renderer2, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { HttpService } from '../../services/http.service';
import { ToastComponent } from '../toast/toast.component';
import { AuthenticationService } from '../../services/authentication.service';

import { Constants } from '../../../shared/Utils/Constants';
import { GlobalDataService } from '../../services/global-data.service';
import { IProtocolYearGet } from '../../../shared/api-contracts/IProtocolYearGet.interface';
import { ModalComponent } from '../modal/modal.component';
import { IModalForm } from '../../../shared/interfaces/IModalForm.interface';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { comparePasswordValidator } from '../../../shared/validator/comparePassword.validator';
import { IModelFormData } from '../../../shared/interfaces/IModalFormData.interface';
import { IChangePasswordPost } from '../../../shared/api-contracts/IChangePasswordPost.interface';
import { LoaderService } from '../../services/loader.service';
import { NotificationsService } from '../../services/notifications.service';
import { SuccessMessage } from '../../../shared/Utils/SuccessMessage';
import { AppNotification } from '../../../shared/models/AppNotification.model';
import { ErrorMessage } from '../../../shared/Utils/ErrorMessage';
import { PasswordModalComponent } from '../password-modal/password-modal.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgbModule, PasswordModalComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private _authService = inject(AuthenticationService);
  private _httpService = inject(HttpService);
  private _loaderService = inject(LoaderService);
  private _notificationService = inject(NotificationsService);
  private _globalDataService = inject(GlobalDataService);
  private _renderer = inject(Renderer2);
  private _router = inject(Router);

  public colorTheme: 'light' | 'dark' = 'dark';
  public auth = this._authService.auth;
  public years: number[] = [];

  public title = signal<string>('Mudar Senha');
  public modalForm = signal<IModalForm[]>([
    {
      tabName: 'Mudar Senha',
      tabId: 'change-password',
      data: [
        {
          type: 'password',
          nameId: 'userPassword',
          label: 'Nova Senha',
          placeholder: '',
        },
        {
          type: 'password',
          nameId: 'confirmPassword',
          label: 'Confirme a Senha',
          placeholder: '',
        },
        {
          type: 'password',
          nameId: 'oldPassword',
          label: 'Senha Antiga',
          placeholder: '',
        },
      ],
    },
  ]);

  public form = signal<FormGroup<any>>(
    new FormGroup(
      {
        userPassword: new FormControl('', Validators.required),
        confirmPassword: new FormControl('', Validators.required),
        oldPassword: new FormControl('', Validators.required),
      },
      { validators: comparePasswordValidator() }
    )
  );

  constructor() {
    effect(() => {
      this.years = this._globalDataService.years();
    });
  }

  public toggleTheme(): void {
    this.colorTheme = this.colorTheme === 'dark' ? 'light' : 'dark';

    this._renderer.setAttribute(
      document.documentElement,
      'data-bs-theme',
      this.colorTheme
    );
  }

  public async getYears(): Promise<void> {
    const response = await this._httpService.makeRequestAsync<
      IProtocolYearGet[]
    >('GET', Constants.PROTOCOL_YEARS_END_POINT);

    if (!response || !response.data) return;
    response.data.forEach((year) => this.years.push(year.year));
  }

  public onChangeYear(event: Event): void {
    const year = (event.target as HTMLSelectElement).value;

    if (!year) return;

    this._globalDataService.setYear(Number(year));
  }

  public onLogout(): void {
    this._authService.logout();
  }

  public onModalSave = async (form: FormGroup): Promise<void> => {
    try {
      this._loaderService.setLoading();
      const userId = this._authService.auth().user?.userId;
      if (!userId) return;
      const body: IChangePasswordPost = {
        userId,
        userPassword: form.get('userPassword')?.value,
        confirmPassword: form.get('confirmPassword')?.value,
        oldPassword: form.get('oldPassword')?.value,
      };

      const response = await this._httpService.makeRequestAsync(
        'POST',
        `${Constants.AUTH_END_POINT}/ChangePassword`,
        body
      );

      if (!response?.error)
        this._notificationService.openNotification(
          new AppNotification(SuccessMessage.changedPassword, 'success')
        );
    } catch (error) {
      this._notificationService.openNotification(
        new AppNotification(ErrorMessage.changedPassword, 'error')
      );
    } finally {
      this._loaderService.setLoading();
    }
  };

  public toggleIcon(input: IModelFormData) {
    const newForm = this.modalForm().map((modal): IModalForm => {
      const data = modal.data.map((d): IModelFormData => {
        if (d.nameId === input.nameId)
          return {
            ...input,
            type: input.type === 'password' ? '' : 'password',
          };
        return d;
      });
      return { ...modal, data };
    });

    this.modalForm.set(newForm);
  }

  public get toSelectYear(): boolean {
    return (
      this._router.url.includes('protocolo') ||
      this._router.url.includes('caixa')
    );
  }

  public get isLogin(): boolean {
    return this._authService.auth().isLogin;
  }

  public get name(): string {
    return this._authService.auth().user?.name!;
  }
}
