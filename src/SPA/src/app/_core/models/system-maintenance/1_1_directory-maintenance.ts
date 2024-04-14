export interface System_Directory {
  id: number;
  seq: string;
  directoryCode: string;
  directoryName: string;
  slug: string;
  parentDirectoryCode: string;
  language: string;
  updateBy: string;
  updateTime: string | null;
}

export interface DirectoryMaintenanceParam {
  directoryCode: string;
  directoryName: string;
  parentDirectoryCode: string;
}

export interface System_DirectorySource {
  currentPage: number;
  isEdit: boolean;
  status: boolean;
  param: DirectoryMaintenanceParam,
  source?: System_Directory
}
