export interface IPermissionDetails {
  roleName: string;
  protocol: 'Leitura' | 'Escrita' | 'Sem Permissão';
  client: 'Leitura' | 'Escrita' | 'Sem Permissão';
  property: 'Leitura' | 'Escrita' | 'Sem Permissão';
  cashFlow: 'Leitura' | 'Escrita' | 'Sem Permissão';
  partner: 'Leitura' | 'Escrita' | 'Sem Permissão';
  users: 'Leitura' | 'Escrita' | 'Sem Permissão';
  chemical: 'Leitura' | 'Escrita' | 'Sem Permissão';
  details: {
    permissionId: number;
    roleId: number;
  };
}
