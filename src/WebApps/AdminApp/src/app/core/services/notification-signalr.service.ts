import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationSignalRService {
  private hubConnection?: signalR.HubConnection;
  private notificationSubject = new BehaviorSubject<any>(null);
  public notification$ = this.notificationSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  constructor() {}

  public startConnection(): void {
    const token = localStorage.getItem('token');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalrHubUrl, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected');
        this.registerHandlers();
      })
      .catch(err => console.error('SignalR Connection Error: ', err));

    this.hubConnection.onreconnected(() => {
      console.log('SignalR Reconnected');
    });
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveNotification', (notification: any) => {
      console.log('Notification received:', notification);
      this.notificationSubject.next(notification);
      this.incrementUnreadCount();
    });

    this.hubConnection.on('UpdateUnreadCount', (count: number) => {
      this.unreadCountSubject.next(count);
    });
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  private incrementUnreadCount(): void {
    const currentCount = this.unreadCountSubject.value;
    this.unreadCountSubject.next(currentCount + 1);
  }

  public updateUnreadCount(count: number): void {
    this.unreadCountSubject.next(count);
  }
}
