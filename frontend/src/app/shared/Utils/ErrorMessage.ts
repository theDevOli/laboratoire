import { NotificationMessage } from '../models/NotificationMessage.model';

export class ErrorMessage {
  public static unauthorized = new NotificationMessage(
    'Credenciais inválidas!',
    'Usuário ou senha estão incorretos.'
  );

  public static inactive = new NotificationMessage(
    'Erro de Autenticação!',
    'Conta inativa. Entre em contato com o suporte para mais informações.'
  );

  public static refreshToken = new NotificationMessage(
    'Erro de Autenticação!',
    'Sessão expirada. Por favor, faça o login novamente.'
  );

  public static fetchError = new NotificationMessage(
    'Algo deu errado. Por favor, tente novamente.',
    'Ocorreu um erro inesperado ao processar sua solicitação. Isso pode ser devido a um problema com o servidor ou o recurso solicitado não pôde ser encontrado. Tente novamente mais tarde ou entre em contato com o suporte, se o problema persistir'
  );

  public static noReport = new NotificationMessage(
    'Laudo sem dados!',
    'O laudo ainda não foi digitado. Digite-o primeiro para depois realizar o download.'
  );

  public static noPaidReport = new NotificationMessage(
    'Laudo com pendências!',
    'O laudo ainda possui pendências. Por favor, entrar em contato.'
  );

  public static waitReport = new NotificationMessage(
    'Laudo sem dados!',
    'O laudo ainda não foi digitado. Por favor, aguarde.'
  );

  public static changedPassword = new NotificationMessage(
    'Erro!',
    'A Senha não foi alterada, por favor verifique se a Senha antiga esta correta.'
  );

  public static resetPassword = new NotificationMessage(
    'Erro!',
    'A Senha não foi alterada, por favor verifique se a Senha antiga esta correta.'
  );
}
