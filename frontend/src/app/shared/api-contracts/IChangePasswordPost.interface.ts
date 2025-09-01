export interface IChangePasswordPost {
  userId: string;
  userPassword: string;
  confirmPassword: string;
  oldPassword: string;
}
