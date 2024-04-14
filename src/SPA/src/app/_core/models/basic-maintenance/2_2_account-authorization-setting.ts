export interface App_AccountDTO {
  account: string;
  email: string;
  password: string;
  againNewPassword: string;
  fullName: string;
  isActive: boolean;
  isActivestr: string;
  role: string;
  passwordReset: boolean;
  listRole: string[];
  updateBy: string;
  updateTime: string | null;
  updateTimeStr: string;
  lang: string
}

export interface AccountAuthorizationSettingParam {
  account: string;
  email: string;
  password: string;
  againNewPassword: string;
  fullName: string;
  isActive: string | boolean;
  isActiveStr: string;
  role: string;
  updateBy: string;
  listRole: string[];
  updateTime: string;
  lang: string
  passwordReset: boolean;
}

export interface App_Account_Source {
  currentPage: number,
  param: AccountAuthorizationSettingParam,
  isEdit: boolean;
  basicCode: App_AccountDTO
}
