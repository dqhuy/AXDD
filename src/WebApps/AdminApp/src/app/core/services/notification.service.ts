import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = environment.notificationApiUrl;

  constructor(private http: HttpClient) {}

  getMyNotifications(params?: any): Observable<PagedResult<any>> {
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

  getUnreadCount(): Observable<number> {
    return this.http.get<ApiResponse<number>>(`${this.apiUrl}/unread-count`)
      .pipe(map(response => response.data!));
  }

  markAsRead(id: string): Observable<boolean> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/${id}/read`, {})
      .pipe(map(response => response.success));
  }

  markAllAsRead(): Observable<boolean> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/read-all`, {})
      .pipe(map(response => response.success));
  }

  deleteNotification(id: string): Observable<boolean> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.success));
  }
}
