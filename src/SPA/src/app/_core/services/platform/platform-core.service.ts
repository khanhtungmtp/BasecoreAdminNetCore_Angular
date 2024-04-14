import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { MenuItem } from '@models/core/menuItem';
import { ServiceResult } from '@models/core/service-result';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { CodeLang, DirectoryInfomation, RoleInfomation } from '@models/auth/auth';

@Injectable({
  providedIn: 'root'
})
export class PlatformCoreService {
  nav: MenuItem[] = []
  constructor(private http: HttpClient) { }

  /**
   * reseu tab
   */
  getDesktop(): { title: string, module: string, power: string, isSelect: boolean } {
    const tabItem = { title: 'Dashboard', module: '/admin/dashboard', power: '', isSelect: true };
    return tabItem;
  }

  /**
   * gen menu
   */
  getNav() {
    this.nav = [];
    const lang: string = localStorage.getItem(LocalStorageConstants.LANG) ?? 'vi_VN';

    const code_lang: CodeLang[] = JSON.parse(localStorage.getItem(LocalStorageConstants.ROLE_LANG) ?? '[]');
    const user_directory: DirectoryInfomation[] = JSON.parse(localStorage.getItem(LocalStorageConstants.DIRECTORY) ?? '[]');
    const user_roles: RoleInfomation[] = JSON.parse(localStorage.getItem(LocalStorageConstants.ROLES) ?? '[]')

    user_directory.filter(x => x.directorySlug != null).forEach((_directory, _directoryIndex) => {
      const foundCodeLang = code_lang.find(val => val.code == _directory.directoryCode && val.lang == lang);
      const directoryNameLang = foundCodeLang
        ? foundCodeLang.name
        : _directory.directoryName;
      let navParent: MenuItem = {
        title: `${_directory.seq}. ${directoryNameLang}`,
        icon: _directory.icon,
        url: _directory.directorySlug,
        children: [],
      };
      user_roles
        .filter(_program => _program.parentDirectoryCode == _directory.directoryCode)
        .forEach(_program => {
          const foundCodeLang = code_lang.find(val => val.code === _program.programCode && val.lang === lang);
          const roleNameLang = foundCodeLang
            ? foundCodeLang.name
            : _program.programName;
          const directoryPath = _directory.parentDirectoryCode.includes('Settings') ? 'settings/' : '';
          const navItem: MenuItem = {
            title: `${_program.programCode} ${roleNameLang}`,
            url: directoryPath + _directory.directorySlug + '/' + _program.slug
          };
          if (!navParent.children) {
            navParent.children = [];
          }
          navParent.children.push(navItem);
        });
      this.nav.push(navParent);
    })
    return this.nav.filter(item => item.children && item.children.length > 0);
  }

  convertUrl(str: string): string {
    const strConvert = str.toLowerCase().replace('/', '-').split(' ').join("-");
    return strConvert;
  }

  /**
   * getUserInfo
   */
  getUserInfo(): void {

  }

}
