import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '@env/environment';
import { NzSafeAny } from 'ng-zorro-antd/core/types';
import { NzMessageService } from 'ng-zorro-antd/message';
import * as qs from 'qs';
import { HttpCustomConfig, OperationResult } from '../utilities/operation-result';

@Injectable({
  providedIn: 'root'
})
export class BaseHttpService {
  uri: string;
  http = inject(HttpClient);
  message = inject(NzMessageService);

  protected constructor() {
    this.uri = !environment.production ? environment.apiUrl : '/site/api';
  }

  // Modify your service methods
  get<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    const params = new HttpParams({ fromString: qs.stringify(param) });
    return this.http.get<OperationResult>(reqPath, { params }).pipe(
      map(response => this.handleResponse(response))
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
      map(response => this.handleResponse(response))
    );
  }

  post<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    return this.http.post<OperationResult>(reqPath, param).pipe(
      map(response => this.handleResponse(response))
    );
  }

  put<T>(path: string, param?: NzSafeAny, config?: HttpCustomConfig): Observable<T> {
    config = config || { needSuccessInfo: false };
    let reqPath = this.getUrl(path, config);
    return this.http.put<OperationResult>(reqPath, param).pipe(
      map(response => this.handleResponse(response))
    );
  }

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
      throw new Error(response.message);
    }
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
