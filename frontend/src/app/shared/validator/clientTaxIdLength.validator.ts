import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function clientTaxIdLengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;

    if (!value) return null;

    const isValid = value.length === 14 || value.length === 18;

    return isValid
      ? null
      : {
          invalidLength: {
            requiredLength: [14, 18],
            actualLength: value.length,
          },
        };
  };
}
