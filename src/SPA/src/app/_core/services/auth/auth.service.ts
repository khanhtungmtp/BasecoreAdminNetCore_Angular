import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { environment } from '@env/environment';

import { UserForLogged, UserForLoggedIn, UserLoginParam } from '@models/auth/auth';
import { map } from 'rxjs/operators';
import { BaseService } from '../platform/baseservice';
import { BaseHttpService } from '../base-http.service';
import { WindowService } from '../common/window.service';
import { LoginInOutService } from './login-in-out.service';
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  // public userLoggedIn;
  public profileUser: UserForLoggedIn | undefined = <UserForLoggedIn>{};
  apiUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  constructor(private http: HttpClient,
    private httpBase: BaseHttpService,
    private windowServe: WindowService,
    private loginOutService: LoginInOutService,
    private router: Router) { super() }

  login(param: UserLoginParam) {
    localStorage.clear();
    return this.httpBase.post<UserForLoggedIn>('Auth/login', param).pipe(map(response => {
      localStorage.setItem(LocalStorageConstants.USER, JSON.stringify(response));
      localStorage.setItem(LocalStorageConstants.TOKEN, response?.token as string);
      return response
    }))
  }

  getPasswordReset(Account: string) {
    let params = new HttpParams().set('Account', Account);
    return this.http.get<Boolean>(this.apiUrl + 'Auth/GetPasswordReset', { params });
  }

  logout(): void {
    this.windowServe.clearStorage();
    this.windowServe.clearSessionStorage();
    this.router.navigate([UrlRouteConstants.LOGIN]);
    this.loginOutService.loginOut().then();
  }

  loggedIn() {
    const token: string = localStorage.getItem(LocalStorageConstants.TOKEN) as string;
    const user: UserForLogged = JSON.parse(localStorage.getItem(LocalStorageConstants.USER) as string);
    return !!user || !this.jwtHelper.isTokenExpired(token);
  }
}
