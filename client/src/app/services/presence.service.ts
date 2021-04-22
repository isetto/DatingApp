import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl
  private hubConnection: HubConnection
  private onlineUsersSource = new BehaviorSubject<string[]>([])
  onlineUsers$: Observable<string[]> = this.onlineUsersSource.asObservable()

  constructor(private toast: ToastrService) { }

  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(`${this.hubUrl}presence`, 
    {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection
    .start()
    .catch(error => console.log(error))

    this.hubConnection.on("UserIsOnline", username=>{
      this.toast.info(`${username} connected`)
    })

    this.hubConnection.on("UserIsOffline", username=>{
      this.toast.warning(`${username} has disconnected`)
    })

    this.hubConnection.on("GetOnlineUsers", (usernames: string[])=>{
      this.onlineUsersSource.next(usernames)
      console.log(usernames)
    })

  }

  stopHubConnection(){
    this.hubConnection.stop().catch(error => console.log(error))
  }
}
