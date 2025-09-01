export interface IPermissionPut {
    permissionId: number;
    roleId: number;
    protocol: boolean | null;
    client: boolean | null;
    property: boolean | null;
    cashFlow: boolean | null;
    partner: boolean | null;
    users: boolean | null;
    chemical: boolean | null;
  }