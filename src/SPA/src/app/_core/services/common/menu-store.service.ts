import { Injectable } from '@angular/core';
import { FunctionTreeVM } from '@app/_core/models/system/functionvm';
import { FunctionUtility } from '@app/_core/utilities/function-utility';
import { environment } from '@env/environment';
import { BehaviorSubject, Observable } from 'rxjs';
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
  constructor(private httpBase: BaseHttpService,) {
    super()
  }
  baseUrl: string = environment.apiUrl;
  private menuArray$ = new BehaviorSubject<FunctionTreeVM[]>([]);

  setMenuArrayStore(menuArray: FunctionTreeVM[]): void {
    this.menuArray$.next(menuArray);
  }

  getMenuArrayStore(): Observable<FunctionTreeVM[]> {
    return this.menuArray$.asObservable();
  }

  getMenuByUserId(userId: string) {
    return this.httpBase.get<FunctionTreeVM[]>(`Users/${userId}/menu-tree`);
  }
}
