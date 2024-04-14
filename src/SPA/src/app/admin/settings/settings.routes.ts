import { Routes } from '@angular/router';
import { SettingsComponent } from './settings.component';
import { S11DirectoryMaintenanceComponent } from './s-1-1-directory-maintenance/s-1-1-directory-maintenance.component';

export const SETTINGS_ROUTES: Routes = [
  {
    path: '',
    component: SettingsComponent,
  },
  {
    path: 'system-maintenance/directory-maintenance',
    component: S11DirectoryMaintenanceComponent,
    data: {
      title: 'Directory Maintenance',
      headerDisplay: "none"
    }
  },
];
