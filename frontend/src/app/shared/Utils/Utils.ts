import { ApiResponse } from '../models/ApiResponse.model';

export class Utils {
  static isRequestFailure(response: ApiResponse<any> | null): boolean {
    return (
      response === null || (response?.data === null && response?.error !== null)
    );
  }

  static toCash(num: number | null): string {
    if (!num) return 'R$0,00';
    if (typeof num === 'string') return `R$${num}`;
    return num.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  static taxFormatter(tax: string, isPlain: boolean = false): string {

    if (!tax) return '';

    tax = tax.replace(/\D/g, '');

    if (isPlain) return tax;

    const length = tax.length;

    if (length === 0) return '';

    if (length <= 3) return tax;

    if (length <= 6) return `${tax.substring(0, 3)}.${tax.substring(3)}`;

    if (length <= 9)
      return `${tax.substring(0, 3)}.${tax.substring(3, 6)}.${tax.substring(
        6
      )}`;

    if (length <= 11)
      return `${tax.substring(0, 3)}.${tax.substring(3, 6)}.${tax.substring(
        6,
        9
      )}-${tax.substring(9)}`;

    if (length <= 12)
      return `${tax.substring(0, 2)}.${tax.substring(2, 5)}.${tax.substring(
        5,
        8
      )}/${tax.substring(8, 12)}`;

    // if (length <= 14)
    return `${tax.substring(0, 2)}.${tax.substring(2, 5)}.${tax.substring(
      5,
      8
    )}/${tax.substring(8, 12)}-${tax.substring(12)}`.substring(0, 18);
  }

  static postalCodeFormatter(
    postalCode: string,
    isPlain: boolean = false
  ): string {
    if (!postalCode) return '';

    postalCode = postalCode.replace(/\D/g, '');

    if (isPlain) return postalCode;

    const length = postalCode.length;

    if (length <= 5) return postalCode.replace(/(\d{2})(\d{1,3})/, '$1.$2');

    return postalCode
      .replace(/(\d{2})(\d{3})(\d{1,3})/, '$1.$2-$3')
      .substring(0, 10);
  }

  public static ccirFormatter(ccir: string, isPlain: boolean = false): string {
    if (!ccir) return '';

    ccir = ccir.replace(/\D/g, '');

    if (isPlain) return ccir;

    const length = ccir.length;

    if (length <= 3) return ccir;
    if (length <= 6) return ccir.replace(/(\d{3})(\d{1,3})/, '$1.$2');
    if (length <= 9) return ccir.replace(/(\d{3})(\d{3})(\d{1,3})/, '$1.$2.$3');
    if (length <= 12)
      return ccir.replace(/(\d{3})(\d{3})(\d{3})(\d{1,3})/, '$1.$2.$3.$4');

    return ccir
      .replace(/(\d{3})(\d{3})(\d{3})(\d{3})(\d{1})/, '$1.$2.$3.$4-$5')
      .substring(0, 17);
  }

  public static itrNirfFormatter(
    itrNirf: string,
    isPlain: boolean = false
  ): string {
    if (!itrNirf) return '';

    itrNirf = itrNirf.replace(/[^a-zA-Z0-9]/g, '');

    if (isPlain) return itrNirf;

    const length = itrNirf.length;

    if (length <= 1) return itrNirf;
    if (length <= 4) return itrNirf.replace(/^(.{1})(.{1,3})/, '$1.$2');
    if (length <= 7)
      return itrNirf.replace(/^(.{1})(.{3})(.{1,3})/, '$1.$2.$3');

    return itrNirf
      .replace(/^(.{1})(.{3})(.{3})(.{1})/, '$1.$2.$3-$4')
      .substring(0, 11);
  }

  static phoneFormatter(phone: string, isPlain: boolean = false): string {
    if (!phone) return '';

    phone = phone.replace(/\D/g, '');

    if (isPlain) return phone;

    const length = phone.length;

    if (!phone) return '';

    if (length < 3) return phone;

    if (length < 4) return phone.replace(/(\d{2})(\d{1})/, '($1) $2');

    if (phone.length < 8)
      return phone.replace(
        new RegExp(`(\\d{2})(\\d{1})(\\d{1,4})`),
        '($1) $2 $3'
      );

    return phone
      .replace(
        new RegExp(`(\\d{2})(\\d{1})(\\d{4})(\\d{1,4})`),
        '($1) $2 $3-$4'
      )
      .substring(0, 16);
  }

  static dateFormatter(
    dateStr: string | Date,
    isInput: boolean = false
  ): string {
    let date;

    if (typeof dateStr === 'string' && dateStr.includes('/')) {
      const day = dateStr.split('/').at(0);
      const month = dateStr.split('/').at(1);
      const year = dateStr.split('/').at(2);
      return `${year}-${month}-${day}`;
    }

    if (typeof dateStr === 'string') date = new Date(dateStr);
    else date = dateStr;

    const day = `${date.getDate()}`.padStart(2, '0');
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const year = date.getFullYear();
    if (isInput) return `${year}-${month}-${day}`;
    return `${day}/${month}/${year}`;
  }
}
