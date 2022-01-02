import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = "https://localhost:5001/api/"
  private curentUserSource = new ReplaySubject<User>(1);
  curentUserSource$ = this.curentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  login(model: User) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((user : User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.curentUserSource.next(user);
        }
      })
    )
  }

  
  register(model: any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.curentUserSource.next(user);
        }
      })
    )
  }

  setCurrentUser(user:User){
    this.curentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.curentUserSource.next(JSON.parse(localStorage.getItem('user') || '{}'));
    location.reload();
  }
}
