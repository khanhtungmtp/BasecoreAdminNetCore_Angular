import { Injectable, inject } from '@angular/core';
import { BaseHttpService } from '../base-http.service';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { UserVM } from '@app/_core/models/user-manager/uservm';
import { UserSearchRequest } from './../../models/user-manager/usersearchrequest';

@Injectable({
  providedIn: 'root'
})
export class UserManagerService {
  httpBase = inject(BaseHttpService);

  getUsersPaging(pagination: PaginationParam, request: UserSearchRequest) {
    const params = { ...pagination, ...request };
    return this.httpBase.get<PagingResult<UserVM>>('Users', params);
  }

  getById(id: string) {
    return this.httpBase.get<UserVM>(`Users/${id}`);
  }

  add(model: UserVM) {
    return this.httpBase.post<string>('Users', model, { needSuccessInfo: true, typeAction: 'add' });
  }

  edit(id: string, model: UserVM) {
    return this.httpBase.put<string>(`Users/${id}`, model, { needSuccessInfo: true, typeAction: 'edit' });
  }

  delete(id: string) {
    return this.httpBase.delete<string>(`Users/${id}`, { needSuccessInfo: true });
  }

  deleteRange(ids: string[]) {
    return this.httpBase.delete<boolean>('Users/DeleteUsers', ids);
  }

}
