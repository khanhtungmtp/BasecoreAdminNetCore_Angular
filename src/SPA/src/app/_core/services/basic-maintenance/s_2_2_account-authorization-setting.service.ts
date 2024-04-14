// import { Injectable, signal } from '@angular/core';
// import { HttpClient, HttpParams } from '@angular/common/http';
// import { environment } from '@env/environment.development';
// import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
// import { AccountAuthorizationSettingParam, App_AccountDTO, App_Account_Source } from '@models/basic-maintenance/2_2_account-authorization-setting';
// import { OperationResult } from '@utilities/operation-result';
// import { toObservable } from '@angular/core/rxjs-interop'
// import { KeyValueUtility } from '@utilities/key-value-utility';
// @Injectable({
//   providedIn: 'root'
// })
// export class AccountAuthorizationSettingService {
//   constructor(private http: HttpClient) { }
//   apiUrl = environment.apiUrl + 'C22AccountAuthorizationSetting/';

//   //signal
//   basicCodeSource = signal<App_Account_Source>(null);
//   source = toObservable(this.basicCodeSource);
//   setSource = (source: App_Account_Source) => this.basicCodeSource.set(source);

//   getData(pagination: PaginationParam, param: AccountAuthorizationSettingParam) {
//     let params = new HttpParams().appendAll({ ...pagination, ...param });

//     return this.http.get<PaginationResult<App_AccountDTO>>(this.apiUrl + 'GetData', { params })
//   }

//   getListDepartment(division: string, factory: string, language: string) {
//     return this.http.get<KeyValueUtility[]>(this.apiUrl + 'GetListDepartment', { params: { division, factory, language } });
//   }

//   getListDivision(language: string) {
//     return this.http.get<KeyValueUtility[]>(this.apiUrl + 'GetListDivision', { params: { language } });
//   }

//   getListListFactory(language: string) {
//     return this.http.get<KeyValueUtility[]>(this.apiUrl + 'GetListFactory', { params: { language } });
//   }

//   getListRole() {
//     return this.http.get<KeyValueUtility[]>(this.apiUrl + 'GetListRole');
//   }

//   create(param: AccountAuthorizationSettingParam) {
//     return this.http.post<OperationResult>(this.apiUrl + "Create", param);
//   }

//   update(param: AccountAuthorizationSettingParam) {
//     return this.http.put<OperationResult>(this.apiUrl + "Update", param);
//   }

//   download(param: AccountAuthorizationSettingParam) {
//     let params = new HttpParams().appendAll({ ...param });

//     return this.http.get<OperationResult>(this.apiUrl + 'DownloadExcel', { params })
//   }
// }
