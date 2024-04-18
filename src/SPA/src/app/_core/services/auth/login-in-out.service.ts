import { inject, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenKey } from '@app/_core/constants/app.constants';
import { MenuStoreService } from '../common/menu-store.service';
import { SimpleReuseStrategy } from '../common/reuse-strategy';
import { TabService } from '../common/tab.service';
import { WindowService } from '../common/window.service';
import { UrlRouteConstants } from '@app/_core/constants/url-route.constants';

/*
 * sign out
 * */
@Injectable({
  providedIn: 'root'
})
export class LoginInOutService {
  private activatedRoute = inject(ActivatedRoute);
  private tabService = inject(TabService);
  private router = inject(Router);
  private menuService = inject(MenuStoreService);
  private windowServe = inject(WindowService);

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
