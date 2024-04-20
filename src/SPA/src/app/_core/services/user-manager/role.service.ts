import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { NzSafeAny } from 'ng-zorro-antd/core/types';

import { BaseHttpService } from '../base-http.service';
import { RoleVM } from '@app/_core/models/user-manager/rolevm';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';

/*
  *Permissions
  * */
export interface Permission {
  hasChildren: boolean;
  menuName: string;
  code: string;
  fatherId: number;
  id: number;
  menuGrade: number; // level
  permissionVo: Permission[];
  isActive?: boolean; // Whether to fold
  checked: boolean;
}

//Update permission parameter interface
export interface PutPermissionParam {
  permissionIds: string[];
  roleId: number;
}


@Injectable({
  providedIn: 'root'
})
export class RoleService {
  httpBase = inject(BaseHttpService);

  public getRolesDetail(id: number): Observable<RoleVM> {
    return this.httpBase.get(`/role/${id}/`);
  }

  public addRoles(param: RoleVM): Observable<void> {
    return this.httpBase.post('/role/', param);
  }

  public delRoles(ids: number[]): Observable<void> {
    return this.httpBase.post('/role/del/', { ids });
  }

  public editRoles(param: RoleVM): Observable<void> {
    return this.httpBase.put('/role/', param);
  }

  public getPermissionById(id: string): Observable<string[]> {
    return this.httpBase.get(`/permission/${id}/`);
  }

  public updatePermission(param: PutPermissionParam): Observable<NzSafeAny> {
    return this.httpBase.put('/permission/', param);
  }

  getRolesPaging(filter: string = '', pagination: PaginationParam) {
    const params = { ...pagination, filter };
    return this.httpBase.get<PagingResult<RoleVM>>('Roles/GetPaging', params);
  }

  public getRoles() {
    return this.httpBase.get<RoleVM[]>(`Roles/GetAll`);
  }
}
