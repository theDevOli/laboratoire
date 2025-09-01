import { inject, Injectable } from '@angular/core';
import { GlobalDataService } from './global-data.service';

@Injectable({
  providedIn: 'root',
})
export class PropertyNameService {
  private _propertyNames: Record<string, string> = {
    //Protocol
    propertyId: 'Propriedades',
    catalogId: 'Tipo de Laudo',
    entryDate: 'Data de Entrada',
    //Clients
    clientName: 'Nome do Cliente',
    clientTaxId: 'CPF/CNPJ',
    clientId: 'Cliente',
    //Property
    city: 'Cidade',
    stateId: 'Estado',
    propertyName: 'Nome do Imóvel',
    //Cash
    description: 'Descrição',
    totalPaid: 'Total Pago',
    paymentDate: 'Data do Pagamento',
    transactionId: 'Tipo de Transação',
    //Partner
    officeName: 'Escritório',
    partnerEmail: 'Email',
    partnerName: 'Nome do Parceiro',
    partnerPhone: 'Contato',
    username: 'Nome do Usuário',
    //User
    roleId: 'Permissão do Usuário',
    name: 'Nome',
    //Permission
    protocol: 'Protocolo',
    client: 'Clientes',
    property: 'Propriedades',
    cashFlow: 'Caixa',
    partner: 'Parceiros',
    chemical: 'Reagentes Quimico',
    //Change Password
    userPassword: 'Nova Senha',
    confirmPassword: 'Confirme a Senha',
    oldPassword: 'Senha Antiga',
  };
  private _globalDataService = inject(GlobalDataService);

  public getPropertyName(key: string): string {
    if (key.toLowerCase().includes('value')) {
      const parameterId = Number(key.split('Value').at(0));

      return (
        this._globalDataService
          .parameter()
          .find((parameter) => parameter.parameterId === parameterId)
          ?.parameterName ?? key
      );
    }
    return this._propertyNames[key] ?? key;
  }
}
