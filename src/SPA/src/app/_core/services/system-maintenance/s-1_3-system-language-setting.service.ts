import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { SystemLanguageSettingParam, System_Language, SystemLanguageDto } from '@models/system-maintenance/1_3-system-language-setting';
import { OperationResult } from '@utilities/operation-result';
import { Pagination, PaginationResult } from '@utilities/pagination-utility';

@Injectable({
  providedIn: 'root'
})
export class S_1_3_SystemLanguageSettingService {

  constructor(private http: HttpClient) { }
  apiUrl: string = environment.apiUrl + 'C13SystemLanguageSetting/';

  getData(pagination: Pagination, param: SystemLanguageSettingParam) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });

    return this.http.get<PaginationResult<System_Language>>(this.apiUrl + 'GetData', { params })
  }

  getLanguages() {
    return this.http.get<SystemLanguageDto[]>(this.apiUrl + 'GetLanguages')
  }

  add(model: System_Language) {
    return this.http.post<OperationResult>(this.apiUrl + 'Create', model);
  }

  edit(params: System_Language) {
    return this.http.put<OperationResult>(this.apiUrl + "Update", params);
  }

  update(model: System_Language) {
    return this.http.put<OperationResult>(this.apiUrl + 'Update', model);
  }

  delete(languageCode: string) {
    let params = new HttpParams().set('languageCode', languageCode)
    return this.http.delete<OperationResult>(this.apiUrl + "Delete", { params: params });
  }
}

