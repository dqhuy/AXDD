import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class EnterpriseService {
  private apiUrl = environment.enterpriseApiUrl;

  constructor(private http: HttpClient) {}

  getEnterprises(params?: any): Observable<PagedResult<any>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.set(key, params[key].toString());
        }
      });
    }

    return this.http.get<ApiResponse<PagedResult<any>>>(`${this.apiUrl}`, { params: httpParams })
      .pipe(map(response => response.data!));
  }

  getById(id: string): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data!));
  }

  create(data: any): Observable<any> {
    return this.http.post<ApiResponse<any>>(this.apiUrl, data)
      .pipe(map(response => response.data!));
  }

  update(id: string, data: any): Observable<any> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}`, data)
      .pipe(map(response => response.data!));
  }

  delete(id: string): Observable<boolean> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.success));
  }

  getStatistics(): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/statistics`)
      .pipe(map(response => response.data!));
  }
}
