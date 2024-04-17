import { NgTemplateOutlet } from '@angular/common';
import { Component, OnInit, ChangeDetectionStrategy, inject, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ModalOptions } from 'ng-zorro-antd/modal';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';

import { HomeNoticeComponent } from '../home-notice/home-notice.component';
import { UserInfoService } from '@app/_core/services/common/userInfo.service';
import { WindowService } from '@app/_core/services/common/window.service';
import { ScreenLessHiddenDirective } from '@app/admin/shared/directives/screen-less-hidden.directive';
import { ToggleFullscreenDirective } from '@app/admin/shared/directives/toggle-fullscreen.directive';
import { AccountService, UserPsd } from '@app/_core/services/auth/account.service';
import { ModalBtnStatus } from '@app/_core/utilities/base-modal';
import { ChangePasswordService } from '@app/admin/pages/change-password/change-password.service';
import { LockWidgetService } from '@app/admin/tpl/lock-widget/lock-widget.service';
import { LoginInOutService } from '@app/_core/services/auth/login-in-out.service';
import { SearchRouteService } from '@app/admin/tpl/search-route/search-route.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { S_1_3_SystemLanguageSettingService } from '@app/_core/services/system-maintenance/s-1_3-system-language-setting.service';
import { SystemLanguageDto } from '@app/_core/models/system-maintenance/1_3-system-language-setting';
import { LocalStorageConstants } from '@app/_core/constants/local-storage.constants';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { UrlRouteConstants } from '@app/_core/constants/url-route.constants';
import { SystemLanguageService } from '@app/_core/services/system/system-language.service';
import { KeyValuePair } from '@app/_core/utilities/key-value-pair';
import { SystemLanguageVM } from '@app/_core/models/system/systemlanguage';

@Component({
  selector: 'app-layout-head-right-menu',
  templateUrl: './layout-head-right-menu.component.html',
  styleUrls: ['./layout-head-right-menu.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [NgTemplateOutlet, ScreenLessHiddenDirective, NzToolTipModule, NzAvatarModule, NzIconModule, NzButtonModule, ToggleFullscreenDirective, NzDropDownModule, NzBadgeModule, NzMenuModule, HomeNoticeComponent, TranslateModule]
})
export class LayoutHeadRightMenuComponent implements OnInit {
  user!: UserPsd;
  listLanguage: SystemLanguageVM[] = [];
  currentLang: string = '';
  baseImage: string = "../../../../assets/images/lang/";
  private router = inject(Router);
  private changePasswordModalService = inject(ChangePasswordService);
  private loginOutService = inject(LoginInOutService);
  private lockWidgetService = inject(LockWidgetService);
  private windowServe = inject(WindowService);
  private searchRouteService = inject(SearchRouteService);
  private message = inject(NzMessageService);
  private userInfoService = inject(UserInfoService);
  private accountService = inject(AccountService);
  private translate = inject(TranslateService);
  private languageService = inject(SystemLanguageService);
  private cdr = inject(ChangeDetectorRef);

  // lock screen
  lockScreen(): void {
    this.lockWidgetService
      .show({
        nzTitle: this.translate.instant('system.caption.lockScreen'),
        nzStyle: { top: '25px' },
        nzWidth: '520px',
        nzFooter: null,
        nzMaskClosable: true
      })
      .subscribe();
  }

  // change Password
  changePassword(): void {
    this.changePasswordModalService.show({ nzTitle: this.translate.instant('system.caption.changePassword') }).subscribe(({ modalValue, status }) => {
      if (status === ModalBtnStatus.Cancel) {
        return;
      }
      this.userInfoService.getUserInfo().subscribe(res => {
        this.user = {
          id: res.userId,
          oldPassword: modalValue.oldPassword,
          newPassword: modalValue.newPassword
        };
      });
      this.accountService.editAccountPsd(this.user).subscribe(() => {
        this.loginOutService.loginOut().then();
        this.message.success(this.translate.instant('system.message.changePasswordOKMsg'));
      });
    });
  }

  showSearchModal(): void {
    const modalOptions: ModalOptions = {
      nzClosable: false,
      nzMaskClosable: true,
      nzStyle: { top: '48px' },
      nzFooter: null,
      nzBodyStyle: { padding: '0' }
    };
    this.searchRouteService.show(modalOptions);
  }

  goLogin(): void {
    this.windowServe.clearStorage();
    this.windowServe.clearSessionStorage();
    this.message.success(this.translate.instant('system.message.logout'));
    this.router.navigate([UrlRouteConstants.LOGIN]);
    this.cdr.markForCheck();
    // this.loginOutService.loginOut().then();
  }

  switchLang(lang: string) {
    localStorage.setItem(LocalStorageConstants.LANG, lang);
    this.translate.use(lang);
    this.message.info(this.translate.instant('system.caption.switchSuccessful'));
  }

  goPage(path: string): void {
    this.router.navigateByUrl(`/default/page-demo/personal/${path}`);
  }

  getLanguage() {
    this.languageService.getLanguages().subscribe({
      next: (res) => {
        if (res?.data)
          this.listLanguage = res.data;
      },
      error: (e) => {
        throw e
      }
    })
  }

  ngOnInit(): void {
    this.getLanguage();
    this.currentLang = localStorage.getItem(LocalStorageConstants.LANG) ?? 'en_US';
  }
}
