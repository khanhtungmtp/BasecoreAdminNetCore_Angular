import { Route } from '@angular/router';

import { FunctionComponent } from './function.component';

export default [
  {
    path: '',
    component: FunctionComponent,
    title: 'Function',
    data: { key: 'function' }
  }
] as Route[];
