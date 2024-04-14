export interface SYSProgramLanguageParam {
  kind: string;
  code: string;
  languageCode: string;
  name: string;
}
export interface SYSProgramLanguageParamSource {
  currentPage: number;
  isEdit: boolean;
  status: boolean;
  source?: SYSProgramLanguageParam
}
export interface LanguageDTO {
  kind: string;
  code: string;
  detail: LanguageDetail[];
  Account: string;
}
export interface LanguageDetail {
  languageCode: string;
  name: string;
}


