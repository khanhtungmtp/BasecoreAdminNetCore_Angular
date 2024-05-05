import { Injectable } from '@angular/core';
import { SystemLanguageVM } from '@app/_core/models/system/systemlanguage';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { BaseHttpService } from '../base-http.service';

@Injectable({
  providedIn: 'root'
})
export class SystemLanguageService {
  constructor(private httpBase: BaseHttpService,) {
  }

  getLanguagesPaging(filter: string = '', pagination: PaginationParam) {
    const params = { ...pagination, filter };

    return this.httpBase.get<PagingResult<SystemLanguageVM>>('SystemLanguages/list-paging', params);
  }

  getLanguages() {
    return this.httpBase.get<SystemLanguageVM[]>('SystemLanguages/list');
  }

  getByLanguageCode(id: string) {
    return this.httpBase.get<SystemLanguageVM>(`SystemLanguages/${id}`);
  }

  add(request: SystemLanguageVM) {
    return this.httpBase.post<string>('SystemLanguages', request, { needSuccessInfo: true, typeAction: 'add' });
  }

  edit(languageCode: string, request: SystemLanguageVM) {
    return this.httpBase.put<string>(`SystemLanguages/${languageCode}`, request, { needSuccessInfo: true, typeAction: 'edit' });
  }

  updateStatus(languageCode: string, isActive: boolean) {
    return this.httpBase.patch<string>(`SystemLanguages/${languageCode}/UpdateStatus`, isActive, { needSuccessInfo: true, typeAction: 'edit' });
  }

  delete(languageCode: string) {
    return this.httpBase.delete<string>(`SystemLanguages/${languageCode}`);
  }

  deleteRange(languageCodes: string[]) {
    return this.httpBase.delete<boolean>('SystemLanguages/deleteRange', languageCodes);
  }

}
