import { Routes } from '@angular/router';
export const SYSTEM_ROUTES: Routes = [
  { path: 'function', loadChildren: () => import('./function/function.routes') },
  { path: 'user-manager', loadChildren: () => import('./user-manager/user-manager.routes') },

];
