import { Routes } from '@angular/router';
export const SYSTEM_ROUTES: Routes = [
  { path: 'function', loadChildren: () => import('./function/function.routes') },

];
