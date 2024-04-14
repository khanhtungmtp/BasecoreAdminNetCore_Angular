import { Injectable, inject } from '@angular/core';
import { CanMatchFn, Route, Router } from '@angular/router';
import { LocalStorageConstants } from '@constants/local-storage.constants';
import { RoleInfomation } from '@models/auth/auth';
import { AuthService } from '@services/auth/auth.service';
import { CommonService } from './../services/common.service';
import { UrlRouteConstants } from '@constants/url-route.constants';
@Injectable({
  providedIn: 'root',
})
export class AdminGuard {
  private listNav: any[] = [];
  private resetPassAddr: string = '2.8'
  constructor(private router: Router, private commonService: CommonService) {
  }

  canMatch(route: Route, service: AuthService): boolean {
    // console.log('route: ', route);
    let role = route.data ? route.data['role'] : undefined;
    // console.log('role: ', role);

    let roleOfUser: RoleInfomation[] = JSON.parse(localStorage.getItem(LocalStorageConstants.ROLES) ?? '[]');
    let checkRole = roleOfUser.map(x => x.programCode).some(x => x.trim() === role?.trim());
    // let userInfo = JSON.parse(localStorage.getItem(LocalStorageConstants.USER) as string);
    if (checkRole) {
      // if (role != this.resetPassAddr)
      //   service.getPasswordReset(userInfo.Account).subscribe({
      //     next: (passwordReset) => {
      //       if (passwordReset) {
      //         const navConstants = this.listNav;

      //         if (navConstants && navConstants.length > 1 && navConstants[1]?.name) {
      //           const parent = navConstants[1].name.toLowerCase();

      //           const role = roleOfUser.find(x => x.programCode == this.resetPassAddr);

      //           if (role && role.programName) {
      //             const child = role.programName.toLowerCase().replace(' ', '-');
      //             this.router.navigate([`/${parent}/${child}`]);
      //           } else {
      //             console.error('Role information is missing.');
      //           }
      //         } else {
      //           console.error('Invalid navigation constants data.');
      //         }
      //       }
      //     },
      //     error: () => {
      //       this.router.navigate([UrlRouteConstants.LOGIN]);
      //     }
      //   })
      return true;
    } else {
      this.router.navigate([UrlRouteConstants.LOGIN]);
      return false;
    }
  }
}

export const adminGuard: CanMatchFn = (route: Route) => {
  const authService = inject(AuthService);
  return inject(AdminGuard).canMatch(route, authService);
};
