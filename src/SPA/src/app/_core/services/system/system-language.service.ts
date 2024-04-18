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

    return this.httpBase.get<PagingResult<SystemLanguageVM>>('SystemLanguages', params);
  }

  getLanguages() {
    return this.httpBase.get<SystemLanguageVM[]>('SystemLanguages/GetLanguages');
  }

  add(model: SystemLanguageVM) {
    return this.httpBase.post<string>('SystemLanguages', model);
  }

  edit(id: string, model: SystemLanguageVM) {
    return this.httpBase.put<string>(`${id}`, model);
  }

  delete(id: string) {
    return this.httpBase.delete<string>(`${id}`);
  }
}
