import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { DirectoryMaintenanceParam, System_Directory } from '@models/system-maintenance/1_1_directory-maintenance';
import { KeyValuePair } from '@utilities/key-value-pair';
import { OperationResult } from '@utilities/operation-result';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { catchError, retry, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class S_1_1_DirectoryMaintenanceService {
  apiUrl = environment.apiUrl + 'C11DirectoryMaintenance/';

  constructor(private http: HttpClient) { }

  getData(pagination: PaginationParam, param: DirectoryMaintenanceParam) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });
    return this.http.get<PaginationResult<System_Directory>>(this.apiUrl + 'GetData', { params }).pipe(
      retry({ count: 1, delay: 1000, resetOnSuccess: true }),
      catchError(this.handleHttpError)
    );
  }

  private handleHttpError(error: HttpErrorResponse) {
    return throwError(() => error);
  }

  delete(directoryCode: string) {
    let params = new HttpParams().appendAll({ directoryCode })
    return this.http.delete<OperationResult>(this.apiUrl + 'Delete', { params });
  }

  add(model: System_Directory) {
    return this.http.post<OperationResult>(this.apiUrl + 'Add', model)
  }

  edit(model: System_Directory) {
    return this.http.put<OperationResult>(this.apiUrl + 'Update', model)
  }
}
