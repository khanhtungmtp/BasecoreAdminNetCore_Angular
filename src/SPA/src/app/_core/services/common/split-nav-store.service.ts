import { Injectable } from '@angular/core';
import { MenuVM } from '@app/_core/models/system/menuvm';
import { BehaviorSubject, Observable } from 'rxjs';

/*** When automatically splitting the menu, the store on the left menu
 */
@Injectable({
  providedIn: 'root'
})
export class SplitNavStoreService {
  private splitLeftNavArray$ = new BehaviorSubject<MenuVM[]>([]);

  setSplitLeftNavArrayStore(menu: MenuVM[]): void {
    this.splitLeftNavArray$.next(menu);
  }

  getSplitLeftNavArrayStore(): Observable<MenuVM[]> {
    return this.splitLeftNavArray$.asObservable();
  }
}
