import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function comparePasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const userPassword = control.get('userPassword')?.value as string;
    const confirmPassword = control.get('confirmPassword')?.value as string;

    if (!userPassword || !confirmPassword) return null;

    const isValid = userPassword === confirmPassword;

    return isValid
      ? null
      : {
          invalidPassword: {
            differentPasswords: true,
          },
        };
  };
}
