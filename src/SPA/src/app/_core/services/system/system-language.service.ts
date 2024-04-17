import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { SystemLanguageVM } from '@app/_core/models/system/systemlanguage';
import { KeyValuePair } from '@app/_core/utilities/key-value-pair';
import { OperationResult } from '@app/_core/utilities/operation-result';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { environment } from '@env/environment';

@Injectable({
  providedIn: 'root'
})
export class SystemLanguageService {
  baseUrl: string = environment.apiUrl + 'SystemLanguages';
  constructor(private http: HttpClient) {
  }
  getLanguagesPaging(filter: string = '', pagination: PaginationParam) {
    const params = new HttpParams()
      .appendAll({ ...pagination, filter });

    return this.http.get<OperationResult<PagingResult<SystemLanguageVM>>>(`${this.baseUrl}`, { params });
  }

  getLanguages() {
    return this.http.get<OperationResult<SystemLanguageVM[]>>(this.baseUrl + '/GetLanguages');
  }

  add(model: SystemLanguageVM) {
    return this.http.post<OperationResult<string>>(`${this.baseUrl}`, model);
  }

  edit(id: string, model: SystemLanguageVM) {
    return this.http.put<OperationResult<string>>(`${this.baseUrl}/${id}`, model);
  }

  deleteFunction(id: string) {
    return this.http.delete<OperationResult<string>>(`${this.baseUrl}/${id}`);
  }
}
