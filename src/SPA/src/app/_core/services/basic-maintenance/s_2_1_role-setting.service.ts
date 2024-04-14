// import { ProgramGroupParam, RoleSetting, RoleSettingEdit, RoleSettingPut, RoleSettingPutEdit } from '@models/basic-maintenance/2_1_role-setting';
// import { OperationResult } from '@utilities/operation-result';
// import { BehaviorSubject, Observable } from 'rxjs';
// import { environment } from '@env/environment.development';
// import { HttpClient, HttpParams } from '@angular/common/http';
// import { Injectable, inject } from '@angular/core';
// import { ResolveFn } from '@angular/router';
// import { Pagination, PaginationResult } from '@utilities/pagination-utility';
// import { RoleSettingSearchDetail, RoleSettingAdd } from '@models/basic-maintenance/2_1_role-setting';
// import { KeyValuePair } from '@utilities/key-value-pair';
// import { TreeviewItem } from '@ash-mezdo/ngx-treeview';

// @Injectable({
//   providedIn: 'root',
// })

// export class S_2_1_Role_Setting {
//   baseUrl = `${environment.apiUrl}C21RoleSetting/`;
//   paramSearch = new BehaviorSubject<RoleSetting>(null);
//   currentParamSearch = this.paramSearch.asObservable();
//   paramForm = new BehaviorSubject<string>(null);
//   currentParamForm = this.paramForm.asObservable();
//   constructor(private http: HttpClient) { }

//   setParamSearch = (item: RoleSetting) =>
//     this.paramSearch.next(item);
//   setParamForm = (item: string) =>
//     this.paramForm.next(item);

//   getRoleDropDownList() {
//     return this.http.get<KeyValuePair[]>(`${this.baseUrl}GetRoleDropDownList`);
//   }
//   getSearchDetail(
//     param: Pagination,
//     filter: RoleSetting
//   ): Observable<PaginationResult<RoleSettingSearchDetail>> {
//     let params = new HttpParams().appendAll({ ...param, ...filter });
//     return this.http.get<PaginationResult<RoleSettingSearchDetail>>(
//       `${this.baseUrl}GetSearchDetail`,
//       { params }
//     );
//   }
//   getProgramGroup(param: ProgramGroupParam) {
//     let params = new HttpParams().appendAll({ ...param });
//     return this.http.get<TreeviewItem[]>(`${this.baseUrl}GetProgramGroup`, {
//       params,
//     });
//   }

//   getProgramGroupList(role: string) {
//     return this.http.get<string[]>(`${this.baseUrl}GetProgramGroupList`);
//   }

//   getRoleSettingEdit(param: ProgramGroupParam) {
//     let params = new HttpParams().appendAll({ ...param });
//     return this.http.get<RoleSettingEdit>(`${this.baseUrl}GetRoleSettingEdit`, {
//       params,
//     });
//   }

//   getProgramGroupTemp(lang: string) {
//     let params = new HttpParams().set('lang', lang);
//     return this.http.get<TreeviewItem[]>(`${this.baseUrl}GetProgramGroupTemp`, {
//       params,
//     });
//   }

//   putProgramGroup(addList: string[], removeList: string[]): Observable<OperationResult> {
//     let data = <RoleSettingPut>{ addList: addList, removeList: removeList };
//     return this.http.put<OperationResult>(`${this.baseUrl}PutProgramGroup`, data);
//   }

//   postRole(data: RoleSettingAdd): Observable<OperationResult> {
//     return this.http.post<OperationResult>(`${this.baseUrl}PostRole`, data);
//   }

//   putRole(data: RoleSettingPutEdit): Observable<OperationResult> {
//     return this.http.post<OperationResult>(`${this.baseUrl}PutRole`, data);
//   }

//   downloadExcel(param: RoleSetting) {
//     let params = new HttpParams().appendAll({ ...param });
//     return this.http.get<OperationResult>(` ${this.baseUrl}DownloadExcel`, { params });
//   }

//   deleteRole(role: string) {
//     let params = new HttpParams().appendAll({ role })
//     return this.http.delete<OperationResult>(`${this.baseUrl}DeleteRole`, { params });
//   }

// }

// export const roleResolver: ResolveFn<KeyValuePair[]> = () => {
//   return inject(S_2_1_Role_Setting).getRoleDropDownList();
// };
