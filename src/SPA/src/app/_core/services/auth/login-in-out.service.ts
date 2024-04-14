import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenKey, TokenPre } from '@app/_core/constants/app.constants';
import { Menu } from '@app/_core/models/common/types';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { MenuStoreService } from '../common/menu-store.service';
import { SimpleReuseStrategy } from '../common/reuse-strategy';
import { TabService } from '../common/tab.service';
import { UserInfoService, UserInfo } from '../common/userInfo.service';
import { WindowService } from '../common/window.service';
import { LoginService } from './login.service';
import { ActionCode } from '@app/_core/constants/actionCode';
import { fnFlatDataHasParentToTree } from '@app/_core/utilities/treeTableTools';
import { UrlRouteConstants } from '@app/_core/constants/url-route.constants';

/*
 * sign out
 * */
@Injectable({
  providedIn: 'root'
})
export class LoginInOutService {
  private destroyRef = inject(DestroyRef);
  private activatedRoute = inject(ActivatedRoute);
  private tabService = inject(TabService);
  private loginService = inject(LoginService);
  private router = inject(Router);
  private userInfoService = inject(UserInfoService);
  private menuService = inject(MenuStoreService);
  private windowServe = inject(WindowService);

  // Get the menu array by user ID
  getMenuByUserId(userId: number): Observable<Menu[]> {
    return this.loginService.getMenuByUserId(userId);
  }

  loginIn(token: string): Promise<void> {
    return new Promise(resolve => {
      //Cache the token persistently. Please note that if there is no cache, it will be intercepted in the route guard and the route will not be allowed to jump.
      // This route is guarded at src/app/core/services/common/guard/judgeLogin.guard.ts
      this.windowServe.setSessionStorage(TokenKey, TokenPre + token);
      // Parse the token and obtain user information
      const userInfo: UserInfo = this.userInfoService.parsToken(TokenPre + token);
      // todo Here are the permissions for the button to open the details in the manual adding of static page tab operations, because they involve routing jumps and will be guarded, but the permissions are not managed through the backend, so the following two lines manually add permissions, In actual operation, you can delete the following 2 lines
      userInfo.authCode.push(ActionCode.TabsDetail);
      userInfo.authCode.push(ActionCode.SearchTableDetail);
      // Cache user information into the global service
      this.userInfoService.setUserInfo(userInfo);
      // Get the menu owned by this user through the user ID
      this.getMenuByUserId(userInfo.userId)
        .pipe(
          finalize(() => {
            resolve();
          }),
          takeUntilDestroyed(this.destroyRef)
        )
        .subscribe(menus => {
          menus = menus.filter(item => {
            item.selected = false;
            item.open = false;
            return item.menuType === 'C';
          });
          const temp = fnFlatDataHasParentToTree(menus);
          // Storage menu
          this.menuService.setMenuArrayStore(temp);
          resolve();
        });
    });
  }

  // Clearing the Tab cache is something related to route reuse.
  async clearTabCash(): Promise<void> {
    await SimpleReuseStrategy.deleteAllRouteSnapshot(this.activatedRoute.snapshot);
    return await new Promise(resolve => {
      // clear tab
      this.tabService.clearTabs();
      resolve();
    });
  }

  clearSessionCash(): Promise<void> {
    return new Promise(resolve => {
      this.windowServe.removeSessionStorage(TokenKey);
      this.menuService.setMenuArrayStore([]);
      resolve();
    });
  }

  async loginOut(): Promise<void> {
    await this.clearTabCash();
    await this.clearSessionCash();
    this.router.navigate([UrlRouteConstants.LOGIN]);
  }
}
