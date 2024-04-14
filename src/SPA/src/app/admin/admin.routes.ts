import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SETTINGS_ROUTES } from './settings/settings.routes';
import { BLANK_ROUTES } from './layouts/blank/blank.routes';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
    data: {
      title: 'Dashboard',
      headerDisplay: "none"
    }
  },
  {
    path: 'settings',
    children: SETTINGS_ROUTES,
    data: {
      title: 'Settings',
      headerDisplay: "none"
    }
  }
];
