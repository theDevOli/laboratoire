import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'clientes',
        loadComponent: () =>
          import('./pages/client/component/client.component').then(
            (mod) => mod.ClientComponent
          ),
      },
      {
        path: 'home',
        loadComponent: () =>
          import('./pages/home/component/home.component').then(
            (mod) => mod.HomeComponent
          ),
      },
      {
        path: 'protocolo',
        loadComponent: () =>
          import('./pages/protocol/component/protocol.component').then(
            (mod) => mod.ProtocolComponent
          ),
      },
      {
        path: 'propriedades',
        loadComponent: () =>
          import('./pages/property/component/property.component').then(
            (mod) => mod.PropertyComponent
          ),
      },
      {
        path: 'usuarios',
        loadComponent: () =>
          import('./pages/user/component/user.component').then(
            (mod) => mod.UserComponent
          ),
      },
      {
        path: 'permissoes',
        loadComponent: () =>
          import('./pages/permission/componet/permission.component').then(
            (mod) => mod.PermissionComponent
          ),
      },
      {
        path: 'caixa',
        loadComponent: () =>
          import('./pages/cash/component/cash.component').then(
            (mod) => mod.CashComponent
          ),
      },
      {
        path: 'parceiros',
        loadComponent: () =>
          import('./pages/partner/component/partner.component').then(
            (mod) => mod.PartnerComponent
          ),
      },
    ],
    canActivate: [authGuard],
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then((mod) => mod.LoginComponent),
  },
];
