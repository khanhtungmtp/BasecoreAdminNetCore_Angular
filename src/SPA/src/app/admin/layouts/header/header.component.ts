import { Component, OnInit } from '@angular/core';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { SystemLanguageDto } from '@models/system-maintenance/1_3-system-language-setting';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '@services/auth/auth.service';
import { NzNotificationCustomService } from '@services/nz-notificationCustom.service';
import { S_1_3_SystemLanguageSettingService } from '@services/system-maintenance/s-1_3-system-language-setting.service';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NzBadgeModule, NzDropDownModule, NzIconModule, TranslateModule, NzAvatarModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  userName: string | null = ''
  listLanguage: SystemLanguageDto[] = [];
  baseImage: string = "../../../../assets/images/lang/";

  constructor(private authService: AuthService, private languageService: S_1_3_SystemLanguageSettingService,
    private translate: TranslateService, private notification: NzNotificationCustomService) {
    this.userName = this.getAccountFromLocalStorage()
  }

  ngOnInit(): void {
    // this.getLanguage()

  }

  getAccountFromLocalStorage(): string | null {
    const userData = localStorage.getItem(LocalStorageConstants.USER);
    if (userData) {
      const parsedData = JSON.parse(userData);
      return parsedData.account;
    }
    return null;
  }

  logout() {
    this.authService.logout();
    this.notification.info(this.translate.instant('system.message.logout'), this.translate.instant('system.caption.success'));
  }

  getLanguage() {
    this.languageService.getLanguages().subscribe({
      next: (res: SystemLanguageDto[]) => {
        this.listLanguage = res;
      },
      error: (e) => {
        throw e
      }
    })
  }

  switchLang(lang: string) {
    localStorage.setItem(LocalStorageConstants.LANG, lang);
    this.translate.use(lang);
  }

}
