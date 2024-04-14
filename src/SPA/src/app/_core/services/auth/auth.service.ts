import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OperationResult } from '@app/_core/utilities/operation-result';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { environment } from '@env/environment';

import { UserForLogged, UserForLoggedIn, UserLoginParam } from '@models/auth/auth';
import { KeyValuePair } from '@utilities/key-value-pair';
import { catchError, map } from 'rxjs/operators';
import { BaseService } from '../platform/baseservice';
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  // public userLoggedIn;
  public profileUser: UserForLoggedIn | undefined = <UserForLoggedIn>{};
  apiUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  constructor(private http: HttpClient,
    private router: Router) { super() }

  login(param: UserLoginParam) {
    return this.http.post<OperationResult<UserForLoggedIn>>(this.apiUrl + 'Auth/login', param).pipe(map(response => {
      localStorage.setItem(LocalStorageConstants.USER, JSON.stringify(response.data));
      localStorage.setItem(LocalStorageConstants.TOKEN, response?.data?.token as string);
    })
    )
  }

  getPasswordReset(Account: string) {
    let params = new HttpParams().set('Account', Account);
    return this.http.get<Boolean>(this.apiUrl + 'Auth/GetPasswordReset', { params });
  }

  getListFactory() {
    return this.http.get<KeyValuePair[]>(this.apiUrl + 'Auth/GetListFactory');
  }

  logout() {
    localStorage.clear();
    this.router.navigate([UrlRouteConstants.LOGIN]);
  }

  loggedIn() {
    const token: string = localStorage.getItem(LocalStorageConstants.TOKEN) as string;
    const user: UserForLogged = JSON.parse(localStorage.getItem(LocalStorageConstants.USER) as string);
    return !!user || !this.jwtHelper.isTokenExpired(token);
  }
}
