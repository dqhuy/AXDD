import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = environment.reportApiUrl;

  constructor(private http: HttpClient) {}

  getReports(params?: any): Observable<PagedResult<any>> {
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

  getPendingReports(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/pending`)
      .pipe(map(response => response.data!));
  }

  approveReport(id: string, notes?: string): Observable<any> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/approve`, { reviewerNotes: notes })
      .pipe(map(response => response.data!));
  }

  rejectReport(id: string, reason: string): Observable<any> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/reject`, { rejectionReason: reason })
      .pipe(map(response => response.data!));
  }

  submitReport(data: any): Observable<any> {
    return this.http.post<ApiResponse<any>>(this.apiUrl, data)
      .pipe(map(response => response.data!));
  }
}
