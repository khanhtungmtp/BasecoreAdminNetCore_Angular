export interface System_Directory {
  directoryCode: string;
  directoryName: string;
  parentDirectoryCode: string;
  language: string;
  updateBy: string;
  updateTime: string | null;
}
export interface Directory {
  DirectoryCode: string;
  DirectoryName: string;
}
export interface System_Program {
  id: number;
  programCode: string;
  programName: string;
  parentDirectoryCode: string;
  updateBy: string;
  updateTime: string | null;
  function: string;
  listFunction: string[];
}
export interface System_ProgramParam {
  programCode: string;
  programName: string;
  parentDirectoryCode: string;
}
export interface System_Program_Language {
  kind: string;
  code: string;
  languageCode: string;
  name: string;
  updateBy: string;
  updateTime: string | null;
}
export interface System_Program_Function {
  programCode: string;
  fuctionCode: string;
  updateBy: string;
  updateTime: string | null;
}
export interface System_Program_Function_Code {
  fuctionCode: string;
  fuctionNameTW: string;
  fuctionNameEN: string;
  updateBy: string;
  updateTime: string | null;
}
export interface System_ProgramSource {
  currentPage: number;
  isEdit: boolean;
  status: boolean;
  source?: System_Program
}
export interface Function_Program {
  programCode: string;
  fuctionNameTW: string;
  fuctionNameEN: string;
}
export interface Function_All {
  fuctionCode: string;
  fuctionNameTW: string;
  fuctionNameEN: string;
}
