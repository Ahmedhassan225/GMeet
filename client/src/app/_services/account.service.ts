import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl
  private curentUserSource = new ReplaySubject<User|null>(1);
   curentUserSource$ = this.curentUserSource.asObservable();
  constructor(private http: HttpClient, private presence: PresenceService) { 
  }

  login(model: User) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((user : User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  
  register(model: any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if(user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  setCurrentUser(user:User){
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;

    //check if there is a single role or an array of roles
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.curentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.curentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  getDecodedToken(token: string){
    
    //using atob to decode data from Base64 
    return JSON.parse(atob(token.split('.')[1]));
  }
}
