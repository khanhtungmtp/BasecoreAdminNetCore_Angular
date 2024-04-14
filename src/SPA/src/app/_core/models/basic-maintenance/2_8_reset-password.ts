export interface ResetPasswordParam {
    account: string;
    newPassword: string;
}

export interface ResetPasswordModel {
    account: string;
    name: string;
    newPassword: string;
    againNewPassword: string;
}
