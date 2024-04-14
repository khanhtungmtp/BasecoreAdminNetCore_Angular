import { Injectable, ErrorHandler } from "@angular/core";
import { InjectBase } from './inject-base-app';
import { ErrorService } from '@services/error.service';
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorGlobalResponse } from "@models/base/error-global-response";

@Injectable({
  providedIn: "root",
})
export class GlobalErrorHandler extends InjectBase implements ErrorHandler {
  constructor(private errorService: ErrorService) {
    super();
  }

  handleError(error: any) {
    let message: ErrorGlobalResponse = <ErrorGlobalResponse>{};
    if (error instanceof HttpErrorResponse || error.type === 'HttpErrorResponse') {
      // Xử lý lỗi từ phía server
      console.log('error Server', error);
      message = this.errorService.getServerErrorMessage(error);
      this.notification.error('Error: ' + message.statusCode.toString(), message.message)
    } else if (error instanceof ErrorEvent) {
      // Xử lý lỗi từ phía client
      console.log('error Client', error);
      this.notification.error('Error: ' + error.error, error.message)
    } else {
      // Xử lý lỗi không xác định được | loi da custom response apiError
      // Unknown Error client
      console.log('Unknown Error client');
      console.log('error: ', error);
      this.notification.error(`Error dev: ${error.message}`, error.message)
      // }
    }
  }
}
