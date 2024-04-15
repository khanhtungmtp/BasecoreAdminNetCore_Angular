import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { KeyValuePair } from '@app/_core/utilities/key-value-pair';
import { OperationResult } from '@app/_core/utilities/operation-result';
import { PaginationParam, PagingResult } from '@app/_core/utilities/pagination-utility';
import { environment } from '@env/environment';

@Injectable({
  providedIn: 'root'
})
export class FunctionService {
  baseUrl: string = environment.apiUrl + 'Functions';
  constructor(private http: HttpClient) {
  }
  getFunctionsPaging(filter: string = '', pagination: PaginationParam) {
    const params = new HttpParams()
      .appendAll({ ...pagination, filter });

    return this.http.get<OperationResult<PagingResult<FunctionVM>>>(`${this.baseUrl}`, { params });
  }

  getParentIds() {
    return this.http.get<OperationResult<KeyValuePair[]>>(`${this.baseUrl}/parentIds`);
  }

  getById(id: string) {
    return this.http.get<OperationResult<FunctionVM>>(`${this.baseUrl}/${id}`);
  }

  add(model: FunctionVM) {
    return this.http.post<OperationResult<string>>(`${this.baseUrl}`, model);
  }

  edit(id: string, model: FunctionVM) {
    return this.http.put<OperationResult<string>>(`${this.baseUrl}/${id}`, model);
  }

  deleteFunction(id: string) {
    return this.http.delete<OperationResult<string>>(`${this.baseUrl}/${id}`);
  }
}
