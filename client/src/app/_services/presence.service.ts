import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { error } from 'console';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, pipe, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
  constructor(private toastr :ToastrService, private router: Router) { }

  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username =>{
      this.onlineUsers$.pipe(take(1)).subscribe(usernames =>{
        this.onlineUsersSource.next([...usernames,username])
      })
    })

    this.hubConnection.on('UserIsOffline', username =>{
      this.onlineUsers$.pipe(take(1)).subscribe(usernames =>{
        this.onlineUsersSource.next([...usernames.filter(x =>x !== username)])
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) =>{
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on('NewMessageRecieved', ({username, knownAs})=>{
      this.toastr.info(knownAs + 'has sent you a new Message!')
        .onTap.pipe(take(1))
        .subscribe(() =>{
          this.router.navigateByUrl('/members/' + username + '?tab3')
        });
    })
  }

  stopHubConnection(){

    this.hubConnection.stop().catch(error => console.log(error));
  }
}
