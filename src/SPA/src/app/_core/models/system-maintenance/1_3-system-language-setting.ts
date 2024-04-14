export interface SystemLanguageSettingParam {
  languageCode: string;
  languageName: string;
  isActive: boolean;
  updateBy: string;
  updateTime: string;
}
export interface System_Language {
  id: number;
  languageCode: string;
  languageName: string;
  isActive: boolean;
  updateBy: string;
  updateTime: string | null;
}

export interface SystemLanguageSettingData {
  dataList: SystemLanguageSettingParam,
  isEdit: boolean,
  status: boolean
}

export interface System_Language_Source {
  currentPage: number,
  param: SystemLanguageSettingParam,
  isEdit: boolean;
  basicCode: System_Language
}


export interface SystemLanguageDto {
  languageCode: string;
  languageName: string;
  urlImage: string;
}
