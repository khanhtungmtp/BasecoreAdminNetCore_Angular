import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { NzSafeAny } from 'ng-zorro-antd/core/types';
import { NzMessageService } from 'ng-zorro-antd/message';
import qs from 'qs';
import { HttpCustomConfig, OperationResult } from '../utilities/operation-result';
import { Router } from '@angular/router';
import { NzSpinnerCustomService } from './common/nz-spinner.service';

@Injectable({
  providedIn: 'root'
})
export class BaseHttpService {
  uri: string;
  http = inject(HttpClient);
  message = inject(NzMessageService);
  router = inject(Router);
  spinnerService = inject(NzSpinnerCustomService);

  protected constructor() {
    this.uri = !environment.production ? environment.apiUrl : 'https://localhost:6001/api/';
  }

  // Modify your service methods
  get<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    const params = new HttpParams({ fromString: qs.stringify(param) });
    return this.http.get<OperationResult>(reqPath, { params }).pipe(
      map(response => this.handleResponse(response)),
      catchError(this.handleError)
    );
  }

  delete<T>(path: string, body?: NzSafeAny, queryParams?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);

    // Chuyển đổi queryParams thành HttpParams
    const params = queryParams ? new HttpParams({ fromObject: queryParams }) : new HttpParams();

    const options = {
      params: params,
      body: body,
      ...config, // Bổ sung thêm cấu hình nếu có
    };

    // Sử dụng http.delete với RequestOptions bao gồm cả params và body
    return this.http.delete<OperationResult>(reqPath, options).pipe(
      map(response => this.handleResponse(response)),
      catchError(this.handleError)
    );
  }

  post<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    return this.http.post<OperationResult>(reqPath, param).pipe(
      map(response => this.handleResponse(response)),
      catchError(this.handleError)
    );
  }

  put<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    return this.http.put<OperationResult>(reqPath, param).pipe(
      map(response => this.handleResponse(response)),
      catchError(this.handleError)
    );
  }

  downLoadWithBlob(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<NzSafeAny> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    return this.http.post(reqPath, param, {
      responseType: 'blob',
      headers: new HttpHeaders().append('Content-Type', 'application/json')
    });
  }

  getUrl(path: string, config: HttpCustomConfig): string {
    let reqPath = this.uri + path;
    if (config.otherUrl) {
      reqPath = path;
    }
    return reqPath;
  }


  private handleError(error: any) {
    console.log('An error occurred baseServices:', error);

    // Tiếp tục truyền lỗi tới global error handler
    return throwError(() => error);
  }

  // handle success
  private handleResponse<T>(response: OperationResult<T>): T | boolean {
    if (response.succeeded) {
      // Kiểm tra nếu có dữ liệu, trả về dữ liệu đó
      if (response.data !== undefined) {
        return response.data;
      }
      // Nếu không có dữ liệu nhưng vẫn thành công, log hoặc thực hiện một hành động nào đó
      else {
        return true;
      }
    } else {
      throw response.message;
    }
  }


  // resultHandle<T>(config: HttpCustomConfig): (observable: Observable<ActionResult<T>>) => Observable<T> {
  //   return (observable: Observable<ActionResult<T>>) => {
  //     return observable.pipe(
  //       filter(item => {
  //         return this.handleFilter(item, !!config.needSuccessInfo);
  //       }),
  //       map(item => {
  //         if (item.code !== 0) {
  //           throw new Error(item.msg);
  //         }
  //         return item.data;
  //       })
  //     );
  //   };
  // }

  // handleFilter<T>(item: ActionResult<T>, needSuccessInfo: boolean): boolean {
  //   if (item.code !== 0) {
  //     this.message.error(item.msg);
  //   } else if (needSuccessInfo) {
  //     this.message.success('Successful operation');
  //   }
  //   return true;
  // }
}
