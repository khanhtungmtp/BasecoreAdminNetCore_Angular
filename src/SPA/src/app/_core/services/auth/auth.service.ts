import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { TokenRequest } from '@app/_core/models/auth/token-request';
import { UserForLogged, UserForLoggedIn, UserLoginParam } from '@models/auth/auth';
import { map } from 'rxjs/operators';
import { BaseHttpService } from '../base-http.service';
import { WindowService } from '../common/window.service';
import { LoginInOutService } from './login-in-out.service';
import { UserInformation } from '@app/_core/models/auth/userInformation';
import { AuthResponse } from '@app/_core/models/auth/auth-response';
import { BehaviorSubject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public profileUser: UserInformation = <UserInformation>{};
  jwtHelper = new JwtHelperService();
  private isRefreshingToken = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private httpBase: BaseHttpService,
    private windowServe: WindowService,
    private loginOutService: LoginInOutService,
    private router: Router) { }

  login(param: UserLoginParam) {
    return this.httpBase.post<UserForLoggedIn>('Auth/login', param).pipe(map(response => {
      // except 2 field token, refreshToken
      const { token, refreshToken, ...otherProps } = response;
      this.profileUser = { ...otherProps }
      this.setToken(response.token);
      this.setRefreshToken(response.refreshToken);
      this.setUserProfile(this.profileUser);
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

  refreshToken() {
    const tokenRequest: TokenRequest = {
      refreshToken: this.getRefreshToken() as string,
      email: this.getEmail() as string,
      token: this.getToken() as string
    }
    return this.httpBase.post<AuthResponse>('Auth/refresh-token', tokenRequest);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token')
  }

  // handle
  public setToken(token: string): void {
    localStorage.setItem(LocalStorageConstants.TOKEN, token);
  };

  public setRefreshToken(refreshToken: string): void {
    localStorage.setItem(LocalStorageConstants.REFRESH_TOKEN, refreshToken);
  };

  public setUserProfile(user: UserInformation): void {
    localStorage.setItem(LocalStorageConstants.USER, JSON.stringify(user));
  }

  public getRefreshToken = (): string | null => {
    return localStorage.getItem(LocalStorageConstants.REFRESH_TOKEN);
  };

  public getToken = (): string | null => {
    return localStorage.getItem(LocalStorageConstants.TOKEN);
  };

  public getEmail = (): string => {
    const user: UserForLogged = JSON.parse(localStorage.getItem(LocalStorageConstants.USER) as string);
    return user.email as string;
  };

  public getUserProfile = () => {
    const user: UserInformation = JSON.parse(localStorage.getItem(LocalStorageConstants.USER) as string);
    return user;
  };

  public isTokenExpired() {
    return !!this.getToken() && this.jwtHelper.isTokenExpired(this.getToken());
  }
}
