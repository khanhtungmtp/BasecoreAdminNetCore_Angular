import { Injectable, inject } from '@angular/core';
import { BaseHttpService } from '../base-http.service';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { UserVM } from '@app/_core/models/user-manager/uservm';

@Injectable({
  providedIn: 'root'
})
export class UserManagerService {
  httpBase = inject(BaseHttpService);

  getUsersPaging(filter: string = '', pagination: PaginationParam) {
    const params = { ...pagination, filter };
    return this.httpBase.get<PagingResult<UserVM>>('Users', params);
  }

  getById(id: string) {
    return this.httpBase.get<UserVM>(`Users/${id}`);
  }

  add(model: UserVM) {
    return this.httpBase.post<string>('Users', model);
  }

  edit(id: string, model: UserVM) {
    return this.httpBase.put<string>(`Users/${id}`, model);
  }

  delete(id: string) {
    return this.httpBase.delete<string>(`Users/${id}`);
  }

  deleteRange(ids: string[]) {
    return this.httpBase.delete<boolean>('Users/DeleteRange', ids);
  }

}
