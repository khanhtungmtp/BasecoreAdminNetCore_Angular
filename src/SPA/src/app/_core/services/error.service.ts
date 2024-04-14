import { Injectable } from '@angular/core';
import { ErrorGlobalResponse } from '@models/base/error-global-response';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  getClientErrorMessage(error: any): string {
    // console.log('error: ', error);
    // console.log('error.message: ', error.message);
    return error.message ?
      error.message :
      error.toString();
  }

  /**
   * case 1: exception for system c#
   * case 2: exception custom for APIError
   */
  getServerErrorMessage(error: any): ErrorGlobalResponse {
    if (error.error) {
      return { statusCode: error.statusCode, message: error.message, detail: error.detail } as ErrorGlobalResponse
    }
    return { statusCode: error.statusCode, message: error.message, detail: error.detail } as ErrorGlobalResponse
  }
}
