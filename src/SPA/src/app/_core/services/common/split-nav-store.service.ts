import { Injectable } from '@angular/core';
import { Menu } from '@app/_core/models/common/types';
import { BehaviorSubject, Observable } from 'rxjs';

/*** When automatically splitting the menu, the store on the left menu
 */
@Injectable({
  providedIn: 'root'
})
export class SplitNavStoreService {
  private splitLeftNavArray$ = new BehaviorSubject<Menu[]>([]);

  setSplitLeftNavArrayStore(menu: Menu[]): void {
    this.splitLeftNavArray$.next(menu);
  }

  getSplitLeftNavArrayStore(): Observable<Menu[]> {
    return this.splitLeftNavArray$.asObservable();
  }
}
