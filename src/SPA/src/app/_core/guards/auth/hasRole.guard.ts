import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UrlRouteConstants } from '@app/_core/constants/url-route.constants';
import { AuthService } from '@app/_core/services/auth/auth.service';

export const hasRoleGuardFn: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  let actionCode = route.data["actionCode"] as string;
  var loggedInUser = authService.getUserProfile();
  if (Object.keys(loggedInUser).length > 0) {
    var listPermission = loggedInUser.permissions;
    if (listPermission != null && listPermission.length > 0 && listPermission.filter(x => x == actionCode).length > 0)
      return true;
    else {
      router.navigate([UrlRouteConstants.ACCESS_DENIED], {
        queryParams: {
          returnUrl: state.url
        }
      });
      return false;
    }
  }
  else {
    router.navigate([UrlRouteConstants.LOGIN], {
      queryParams: {
        returnUrl: state.url
      }
    });
    return false;
  }
};
