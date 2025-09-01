import { NotificationMessage } from '../models/NotificationMessage.model';

export class SuccessMessage {
  public static protocolAdded = new NotificationMessage(
    'Operação Concluída!',
    'O Protocolo foi adicionado com sucesso.'
  );
  public static protocolUpdated = new NotificationMessage(
    'Operação Concluída!',
    'O Protocolo foi atualizado com sucesso.'
  );

  public static clientAdded = new NotificationMessage(
    'Operação Concluída!',
    'O Cliente foi adicionado com sucesso.'
  );

  public static propertyAdded = new NotificationMessage(
    'Operação Concluída!',
    'A Propriedade foi adicionada com sucesso.'
  );

  public static changedPassword = new NotificationMessage(
    'Operação Concluída!',
    'A Senha foi alterada com sucesso.'
  );

  public static resetPassword = new NotificationMessage(
    'Operação Concluída!',
    'A Senha foi redefinida com sucesso.'
  );
  public static protocolSpotSaver = new NotificationMessage(
    'Operação Concluída!',
    'Vagas cadastratas com sucesso!'
  );
}
