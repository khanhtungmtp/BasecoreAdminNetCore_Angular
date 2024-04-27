import { Injectable } from '@angular/core';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { FunctionUtility } from '@app/_core/utilities/function-utility';
import { environment } from '@env/environment';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { BaseService } from '../platform/baseservice';
import { BaseHttpService } from '../base-http.service';

//Menu store service
@Injectable({
  providedIn: 'root'
})
export class MenuStoreService extends BaseService {
  /**
   *
   */
  constructor(private httpBase: BaseHttpService,
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
    return this.httpBase.get<FunctionVM[]>(`Users/${userId}/menu`).pipe(map(response => {
      if (response.length === 0) {
        // Handle the undefined case, e.g., by returning an empty array or throwing an error
        console.error('Received undefined data');
        return [];
      }
      return this.ultility.UnflatteringForLeftMenu(response);
    }));
  }
}
