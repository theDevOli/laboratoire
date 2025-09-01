export interface IToken {
  userId: string;
  role:string;
  nbf: number;
  exp: number;
  iat: number;
}
