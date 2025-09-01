import { Component, effect, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';

import { LoaderService } from './core/services/loader.service';
import { LoaderComponent } from './core/shell/loader/loader.component';
import { FooterComponent } from './core/shell/footer/footer.component';
import { HeaderComponent } from './core/shell/header/header.component';
import { AuthenticationService } from './core/services/authentication.service';
import { ToastComponent } from './core/shell/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent,
    LoaderComponent,
    FooterComponent,
    ToastComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private readonly _authenticationService = inject(AuthenticationService);
  private readonly _router = inject(Router);

  public isLoading: boolean = true;
  public isLogin: boolean = false;

  constructor(private loaderService: LoaderService) {
    effect(() => {
      this.isLoading = this.loaderService.loading();
      this.isLogin = this._authenticationService.auth().isLogin;
    });
  }

  public get isBackground(): boolean {
    return this._router.url === '/login';
  }
}
