import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Pagination, PaginationResult } from '@utilities/pagination-utility';
import { Function_All, System_Program, System_ProgramParam } from '@models/system-maintenance/1_2-program-maintenance';
import { OperationResult } from '@utilities/operation-result';
import { KeyValuePair } from '@utilities/key-value-pair';

@Injectable({
  providedIn: 'root'
})
export class S_1_2_programMaintenanceService {
  apiUrl: string = `${environment.apiUrl}C12ProgramMaintenance/`;
  constructor(private http: HttpClient) { }

  getData(pagination: Pagination, param: System_ProgramParam) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });
    return this.http.get<PaginationResult<System_Program>>(this.apiUrl + "GetData", { params: params });
  }

  getDirectory() {
    return this.http.get<KeyValuePair[]>(this.apiUrl + "GetDirectory");
  }

  getFunctionAll() {
    return this.http.get<Function_All[]>(this.apiUrl + "GetFunctionAll");
  }

  add(params: System_Program) {
    return this.http.post<OperationResult>(this.apiUrl + "Add", params);
  }

  edit(params: System_Program) {
    return this.http.put<OperationResult>(this.apiUrl + "Edit", params);
  }

  delete(ProgramCode: string) {
    let params = new HttpParams().set('ProgramCode', ProgramCode)
    return this.http.delete<OperationResult>(this.apiUrl + "Delete", { params: params });
  }

}
