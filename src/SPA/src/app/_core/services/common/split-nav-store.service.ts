import { Injectable } from '@angular/core';
import { Menu } from '@app/_core/models/common/types';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { BehaviorSubject, Observable } from 'rxjs';

/*** When automatically splitting the menu, the store on the left menu
 */
@Injectable({
  providedIn: 'root'
})
export class SplitNavStoreService {
  private splitLeftNavArray$ = new BehaviorSubject<FunctionVM[]>([]);

  setSplitLeftNavArrayStore(menu: FunctionVM[]): void {
    this.splitLeftNavArray$.next(menu);
  }

  getSplitLeftNavArrayStore(): Observable<FunctionVM[]> {
    return this.splitLeftNavArray$.asObservable();
  }
}
