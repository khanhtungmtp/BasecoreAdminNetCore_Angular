import { TreeviewItem } from "@ash-mezdo/ngx-treeview";

export interface ProgramGroupParam {
  role: string;
  lang: string;
}

export interface RoleSetting {
  role: string;
  description: string;
}

export interface RoleSettingSearchDetail extends RoleSetting {
  updateBy: string;
  updateTime: Date | null;
  updateTimeStr: string;
}

export interface RoleSettingPut {
  addList: string[];
  removeList: string[];
}

export interface RoleSettingPutEdit extends RoleSetting {
  addList: string[];
  removeList: string[];
}

export interface RoleSettingAdd extends RoleSetting {
  programGroup: string[];
}

export interface RoleSettingEdit extends RoleSetting {
  roleList: TreeviewItem[]
  roleListStr: string[]
}
