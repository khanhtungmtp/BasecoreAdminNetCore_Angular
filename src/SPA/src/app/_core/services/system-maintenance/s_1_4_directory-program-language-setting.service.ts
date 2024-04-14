import { Injectable, signal } from '@angular/core';
import { environment } from '@env/environment';
import { SYSProgramLanguageParamSource, SYSProgramLanguageParam, LanguageDTO } from '@models/system-maintenance/1_4_directory-program-language';
import { HttpClient, HttpParams } from '@angular/common/http';
import { toObservable } from '@angular/core/rxjs-interop';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { OperationResult } from '@utilities/operation-result';
import { Observable } from 'rxjs';
import { KeyValuePair } from '@utilities/key-value-pair';

@Injectable({
  providedIn: 'root'
})
export class S_1_4_directoryProgramLanguageSettingService {
  baseUrl: string = environment.apiUrl + 'C14DirectoryProgramLanguageSetting/';
  //Lưu Param
  programSource = signal<SYSProgramLanguageParamSource>(<SYSProgramLanguageParamSource>{});
  source = toObservable(this.programSource);
  SetSource = (source: SYSProgramLanguageParamSource) => this.programSource.set(source);

  constructor(private http: HttpClient) { }

  getData(pagination: PaginationParam, param: SYSProgramLanguageParam) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });
    return this.http.get<PaginationResult<SYSProgramLanguageParam>>(this.baseUrl + 'GetData', { params });
  }
  delete(kind: string, code: string): Observable<OperationResult> {
    let params = new HttpParams().appendAll({ kind, code })
    return this.http.delete<OperationResult>(this.baseUrl + 'Delete', { params });
  }
  addNew(param: LanguageDTO) {
    return this.http.post<OperationResult>(this.baseUrl + "Add", param);
  }
  edit(param: LanguageDTO) {
    return this.http.put<OperationResult>(this.baseUrl + "Update", param);
  }

  //Add
  getLanguage() {
    return this.http.get<KeyValuePair[]>(this.baseUrl + 'GetLanguage');
  }
  getProgram() {
    return this.http.get<KeyValuePair[]>(this.baseUrl + 'GetProgram');
  }
  getDirectory() {
    return this.http.get<KeyValuePair[]>(this.baseUrl + 'GetDirectory');
  }
  //Edit
  getNameCode(kind: string, code: string,) {
    let params = new HttpParams().appendAll({ kind, code })
    return this.http.get<KeyValuePair[]>(this.baseUrl + 'GetNameCode', { params });
  }
  getDetail(kind: string, code: string,) {
    let params = new HttpParams().appendAll({ kind, code })
    return this.http.get<LanguageDTO>(this.baseUrl + 'GetDetail', { params });
  }

}
