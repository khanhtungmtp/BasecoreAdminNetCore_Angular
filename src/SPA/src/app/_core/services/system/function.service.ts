import { Injectable } from '@angular/core';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { BaseHttpService } from '../base-http.service';

@Injectable({
  providedIn: 'root'
})
export class FunctionService {
  constructor(private httpBase: BaseHttpService,) {
  }

  getFunctionsPaging(filter: string = '', pagination: PaginationParam) {
    const params = { ...pagination, filter };
    return this.httpBase.get<PagingResult<FunctionVM>>('Functions', params);
  }

  getParentIds() {
    return this.httpBase.get<FunctionVM[]>(`Functions/parentids`);
  }

  getById(id: string) {
    return this.httpBase.get<FunctionVM>(`Functions/${id}`);
  }

  add(model: FunctionVM) {
    return this.httpBase.post<string>('Functions', model);
  }

  edit(id: string, model: FunctionVM) {
    return this.httpBase.put<string>(`Functions/${id}`, model);
  }

  delete(id: string) {
    return this.httpBase.delete<string>(`Functions/${id}`);
  }

  deleteRange(ids: string[]) {
    return this.httpBase.delete<boolean>('Functions/DeleteRangeFunction', ids);
  }
}
