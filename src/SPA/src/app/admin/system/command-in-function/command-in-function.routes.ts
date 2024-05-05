import { Route } from '@angular/router';

import { ActionCode } from '@app/_core/constants/actionCode';
import { CommandInFunctionComponent } from './command-in-function.component';

export default [
  {
    path: '',
    component: CommandInFunctionComponent,
    title: 'Command in functions',
    data: {
      title: 'Command in functions',
      actionCode: ActionCode.CommandInFunctionView,
    },
  }
] as Route[];
