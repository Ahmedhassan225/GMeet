import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accoutService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User|null = null;

    this.accoutService.curentUserSource$.pipe(take(1))
    .subscribe(user => currentUser = user);
    if(currentUser) {
        
      request = request.clone({
        setHeaders: {
          authorization: "Bearer " + JSON.parse(localStorage.getItem('user') || '{}').token
        }
      })
    }
    return next.handle(request);
  }
}
