import { Injectable } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  ValidationErrors,
} from '@angular/forms';
import { Observable, of } from 'rxjs';
import { ClientService } from '../../pages/client/service/client.service';

@Injectable({ providedIn: 'root' })
export class ClientTaxIdValidator {
  constructor(private clientService: ClientService) {}

  clientTaxIdValidator(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      const taxId = control.value;

      if (!taxId) return of(null);
      let res;
      this.clientService
        .isClientOnDB(taxId)
        .then((response) => {
          res = response;
        })
        .catch(() => (res = false));
      res;

      if (!res) return of(null);

      return of({ thereIsClient: true });
    };
  }
}
