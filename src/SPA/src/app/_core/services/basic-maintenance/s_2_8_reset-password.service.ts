import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { ResetPasswordParam } from '@models/basic-maintenance/2_8_reset-password';
import { OperationResult } from '@utilities/operation-result';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class S_2_8_resetPasswordService {
  apiUrl: string = environment.apiUrl + 'C28ResetPassword/';
  constructor(private http: HttpClient) { }

  resetPassword(param: ResetPasswordParam): Observable<OperationResult> {
    return this.http.put<OperationResult>(this.apiUrl + 'ResetPassword', param);
  }
}
