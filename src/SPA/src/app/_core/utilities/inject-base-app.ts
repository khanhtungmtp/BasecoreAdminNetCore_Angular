import { TranslateService } from '@ngx-translate/core';
import { inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DestroyService } from "@services/destroy.service";
// import { NgSnotifyService } from '@services/ng-snotify.service';
import { FunctionUtility } from "@utilities/function-utility";
import { NzNotificationCustomService } from '@services/nz-notificationCustom.service';
import { NzSpinnerCustomService } from '@services/common/nz-spinner.service';
// import { NgxSpinnerService } from "ngx-spinner";
export abstract class InjectBase {
  protected router: Router = inject(Router);
  protected route: ActivatedRoute = inject(ActivatedRoute);
  protected translateService: TranslateService = inject(TranslateService);
  // protected spinnerService: NgxSpinnerService = inject(NgxSpinnerService);
  // protected snotifyService: NgSnotifyService = inject(NgSnotifyService);
  protected destroyService: DestroyService = inject(DestroyService);
  protected functionUtility: FunctionUtility = inject(FunctionUtility);
  protected notification: NzNotificationCustomService = inject(NzNotificationCustomService);
  protected spinnerService: NzSpinnerCustomService = inject(NzSpinnerCustomService);
}
