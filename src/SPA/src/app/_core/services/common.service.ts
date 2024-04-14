import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment'
import { KeyValuePair } from '@utilities/key-value-pair';
@Injectable({
  providedIn: 'root'
})
export class CommonService {
  apiUrl = environment.apiUrl + 'Common/';
  constructor(private http: HttpClient) { }

  getParentDirectoryCode() {
    return this.http.get<KeyValuePair[]>(this.apiUrl + 'GetParentDirectoryCode');
  }
}
