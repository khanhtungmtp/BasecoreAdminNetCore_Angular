import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { NzSpinnerCustomService } from '@services/common/nz-spinner.service';
import { ErrorGlobalResponse } from '../models/base/error-global-response';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private spinnerService: NzSpinnerCustomService,
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => this.handleError(error))
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.log('error handleError: ', error);
    const apiError: ErrorGlobalResponse = <ErrorGlobalResponse>{
      message: 'An error occurred while connecting to the server',
      statusCode: 0,
      type: error.name
    };
    // down server or unthorized
    if (error.status === 0 || error.status === 401) {
      apiError.statusCode = error.status;
      if (error.status === 401) {
        apiError.message = error.error?.message || 'Session expired, please log in again.';
        localStorage.clear();
        this.router.navigate([UrlRouteConstants.LOGIN]);
      }
      this.spinnerService.hide();
      return throwError(() => apiError);
    }

    // custom exception error APIError
    if (error.error?.errors) {
      apiError.statusCode = error.status;
      apiError.message = error.error.message;
      const errors = error.error.errors;
      console.log('errors dev: uncorect field parameter: ', errors);
      apiError.message = errors.join(',');

    } else {
      apiError.statusCode = error.status;
      apiError.message = error.error.message ?? error.message;
    }

    this.spinnerService.hide();
    return throwError(() => apiError);
  }
}
