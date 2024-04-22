import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { TokenRequest } from '@app/_core/models/auth/token-request';
import { UserForLogged, UserForLoggedIn, UserLoginParam } from '@models/auth/auth';
import { map } from 'rxjs/operators';
import { BaseService } from '../platform/baseservice';
import { BaseHttpService } from '../base-http.service';
import { WindowService } from '../common/window.service';
import { LoginInOutService } from './login-in-out.service';
import { TokenResponse } from '@app/_core/models/auth/token-response';
import { AuthResponse } from '@app/_core/models/auth/auth-response';
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  public profileUser: UserForLoggedIn = <UserForLoggedIn>{};
  jwtHelper = new JwtHelperService();
  constructor(
    private httpBase: BaseHttpService,
    private windowServe: WindowService,
    private loginOutService: LoginInOutService,
    private router: Router) { super() }

  login(param: UserLoginParam) {
    return this.httpBase.post<UserForLoggedIn>('Auth/login', param).pipe(map(response => {
      this.profileUser = response
      localStorage.setItem(LocalStorageConstants.USER, JSON.stringify(response));
      localStorage.setItem(LocalStorageConstants.TOKEN, response?.token as string);
      return response
    }))
  }

  getPasswordReset(Account: string) {
    return this.httpBase.get<Boolean>('Auth/GetPasswordReset', Account);
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

  refreshToken(tokenRequest: TokenRequest) {
    return this.httpBase.post<TokenResponse>('Auth/refresh-token', tokenRequest);
  }

  public getToken = (): string | null => {
    return localStorage.getItem(LocalStorageConstants.TOKEN);
  };

  getRefreshToken = (): string | null => {
    const user = localStorage.getItem(LocalStorageConstants.USER);
    if (!user) return null;
    const userDetail: AuthResponse = JSON.parse(user);
    return userDetail.refreshToken;
  };

  private isTokenExpired() {
    const token = this.getToken();
    if (!token) return true;
    if (this.jwtHelper.isTokenExpired(token)) this.logout();
    return true;
  }
}
