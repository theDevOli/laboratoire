import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { lastValueFrom, Observable } from 'rxjs';

import { ApiMethods } from '../../shared/types/ApiMethods.type';
import { ApiResponse } from '../../shared/models/ApiResponse.model';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  private _http = inject(HttpClient);

  public makeRequest(
    method: ApiMethods,
    url: string,
    data: any = null,
    params?: Record<string, string | number | boolean>
  ): Observable<HttpResponse<ApiResponse<any>>> {
    let httpParams = new HttpParams();

    if (params) {
      for (const key in params) {
        if (params.hasOwnProperty(key)) {
          httpParams = httpParams.set(key, params[key].toString());
        }
      }
    }
    return this._http.request<ApiResponse<any>>(method, url, {
      body: data,
      params: params,
      observe: 'response',
      reportProgress: false,
      responseType: 'json',
      withCredentials: true,
      transferCache: false,
    });
  }

  public async makeRequestAsync<T>(
    method: ApiMethods,
    url: string,
    data: any = null,
    params?: Record<string, string | number | boolean>
  ): Promise<ApiResponse<T> | null> {
    const response$ = this.makeRequest(method, url, data, params);
    const response = await lastValueFrom(response$);

    return response.body;
  }

  public async getDocumentAsync(
    url: string,
    protocolId: string
  ): Promise<void> {
    const response$ = this._http.get(url, {
      reportProgress: true,
      responseType: 'blob',
    });

    const blob = await lastValueFrom(response$);

    const urlBlob = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = urlBlob;
    a.download = `relatorio-${protocolId}.pdf`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(urlBlob);
    document.body.removeChild(a);
  }
}
