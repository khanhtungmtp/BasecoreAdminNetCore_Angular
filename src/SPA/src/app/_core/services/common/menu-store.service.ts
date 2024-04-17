import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Menu } from '@app/_core/models/common/types';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { FunctionUtility } from '@app/_core/utilities/function-utility';
import { environment } from '@env/environment';
import { BehaviorSubject, Observable, catchError, map } from 'rxjs';
import { BaseService } from '../platform/baseservice';
import { OperationResult } from '@app/_core/utilities/operation-result';

//Menu store service
@Injectable({
  providedIn: 'root'
})
export class MenuStoreService extends BaseService {
  /**
   *
   */
  constructor(private http: HttpClient,
    private ultility: FunctionUtility,) {
    super()
  }
  baseUrl: string = environment.apiUrl;
  private menuArray$ = new BehaviorSubject<FunctionVM[]>([]);

  setMenuArrayStore(menuArray: FunctionVM[]): void {
    this.menuArray$.next(menuArray);
  }

  getMenuArrayStore(): Observable<FunctionVM[]> {
    return this.menuArray$.asObservable();
  }

  getMenuByUserId(userId: string) {
    return this.http.get<OperationResult<FunctionVM[]>>(`${this.baseUrl}Users/${userId}/menu`).pipe(map(response => {
      if (!response.data) {
        // Handle the undefined case, e.g., by returning an empty array or throwing an error
        console.error('Received undefined data');
        return [];
      }
      return this.ultility.UnflatteringForLeftMenu(response.data);
    }));
  }
}
