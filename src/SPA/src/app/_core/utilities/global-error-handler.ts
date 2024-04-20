import { Injectable, ErrorHandler, inject } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorGlobalResponse } from "@models/base/error-global-response";
import { UrlRouteConstants } from "../constants/url-route.constants";
import { NzSpinnerCustomService } from "../services/common/nz-spinner.service";
import { NzNotificationCustomService } from "../services/nz-notificationCustom.service";
import { Router } from "@angular/router";

@Injectable({
  providedIn: "root",
})
export class GlobalErrorHandler implements ErrorHandler {
  private notification = inject(NzNotificationCustomService);
  private spinnerService = inject(NzSpinnerCustomService);
  private router = inject(Router);

  handleError(error: any) {
    console.log('error global: ', error);
    let apiError: ErrorGlobalResponse = <ErrorGlobalResponse>{
      message: 'An error occurred while connecting to the server',
      statusCode: error.status,
      type: error.name
    };

    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        apiError.message = "Cannot connect to the server";
      } else if (error.status === 401) {
        apiError.message = error.error?.message || 'Session expired, please log in again.';
        localStorage.clear();
        this.router.navigate([UrlRouteConstants.LOGIN]);
      } else if (error.error) {
        apiError = {
          message: error.error.message || apiError.message,
          statusCode: error.error.status || apiError.statusCode,
          type: error.error.type || apiError.type
        };
      } else {
        apiError.message = error.message;
      }
    } else {
      // Lỗi client-side hoặc lỗi mạng
      // custom exception error APIError
      if (error.error?.errors) {
        apiError = {
          message: error.error.errors.join(','),
          statusCode: error.error.status,
          type: error.error.type
        }
        console.log('errors dev: uncorect field parameter: ', error);
        return;
      } else {
        console.log('errors dev:', error);
        apiError = {
          message: error.error?.message ?? error.message,
          statusCode: error?.status ?? 400,
          type: error?.name ?? 'Error dev'
        }
      }
    }

    this.spinnerService.hide();
    this.notification.error('Error: ' + apiError.statusCode, apiError.message)
  }
}
