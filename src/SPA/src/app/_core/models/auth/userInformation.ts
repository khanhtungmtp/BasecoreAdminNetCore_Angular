import { DirectoryInfomation, RoleInfomation, RoleInfomationLanguage } from './auth'

export interface UserInformation {
  id: string;
  fullName: string;
  account: string;
  directories: DirectoryInfomation[];
  roles: RoleInfomation[];
  roleAll: RoleInfomation[];
  roleLang: RoleInfomationLanguage[];
}

