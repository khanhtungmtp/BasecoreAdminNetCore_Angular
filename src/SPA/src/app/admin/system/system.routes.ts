import { Routes } from '@angular/router';
import { ActionCode } from '@app/_core/constants/actionCode';
import { hasRoleGuardFn } from '@app/_core/guards/auth/hasRole.guard';
export const SYSTEM_ROUTES: Routes = [
  {
    path: 'function',
    canActivate: [hasRoleGuardFn],
    data: {
      title: 'Function',
      actionCode: ActionCode.FunctionView,
    },
    loadChildren: () => import('./function/function.routes')
  },
  {
    path: 'user-manager',
    data: {
      title: 'User manager',
      actionCode: ActionCode.UserManagerView,
    },
    loadChildren: () => import('./user-manager/user-manager.routes')
  },
  {
    path: 'role',
    data: {
      title: 'Role manager',
      actionCode: ActionCode.RoleManagerView,
    },
    loadChildren: () => import('./role-manager/role-manager.routes')
  },

];
