import { inject, Injectable } from '@angular/core';
import { Menu } from '@app/_core/models/common/types';
import { Observable } from 'rxjs';
import { BaseHttpService } from '../base-http.service';

// import { MENU_TOKEN } from '@config/menu';


export interface UserLogin {
  name: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  http = inject(BaseHttpService);
  // private menus = inject(MENU_TOKEN);

  public login(params: UserLogin): Observable<string> {
    return this.http.post('/login', params, { needSuccessInfo: false });
  }

  public getMenuByUserId(userId: number): Observable<Menu[]> {
    // If it is a static menu, release the following comment
    // return of(this.menus);
    return this.http.get(`/sysPermission/menu/${userId}`);
  }
}
