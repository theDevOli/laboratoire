export interface IPermissionGet {
  permissionId: number;
  roleId: number;
  roleName: string;
  protocol: boolean | null;
  client: boolean | null;
  property: boolean | null;
  cashFlow: boolean | null;
  partner: boolean | null;
  users: boolean | null;
  chemical: boolean | null;
}
