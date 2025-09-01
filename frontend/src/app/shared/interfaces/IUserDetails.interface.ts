export interface IUserDetails {
  isActive: 'Sim' | 'Não';
  name: string;
  username: string;
  roleName: string;
  details: {
    userId: string;
    partnerId: string | null;
    roleId: number;
  };
}
