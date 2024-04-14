import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { FunctionUtility } from '@app/_core/utilities/function-utility';
import { environment } from '@env/environment';
import { catchError, map } from 'rxjs';
import { BaseService } from '../platform/baseservice';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {
  baseUrl: string = environment.apiUrl + 'Users';
  constructor(private http: HttpClient,
    private ultility: FunctionUtility) {
    super();
  }

}
