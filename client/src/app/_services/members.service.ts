import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

/* const httpOptions = {
  headers: new HttpHeaders({
    Authentication: "Bearer " + JSON.parse(localStorage.getItem('user') || '{}').token
  })
} */

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members :Member[] = [];
  constructor(private http :HttpClient) { }

  getMembers(UserParams: UserParams){
    //if(this.getMembers.length > 0)return of(this.members);
    let params = this.getPaginationHeaders(UserParams.pageNumber, UserParams.pageSize);

    params = params.append('minAge', UserParams.minAge.toString());
    params = params.append('maxAge', UserParams.maxAge.toString());
    params = params.append('gender', UserParams.gender);
    params = params.append('orderBy', UserParams.orderBy);

    return this.getPaginationResult<Member[]>(this.baseUrl + 'users', params);
  }

  private getPaginationResult<T>(url :string, params: HttpParams) {

    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') || '{}');
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }

  getMember(username: string){
    const member = this.members.find(x => x.userName === username);
    if(member !== undefined) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/'+username);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl+'users', member).pipe(
      map(() =>{
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  updateMainphoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/'+ photoId,{});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/'+ photoId,{});
  }
}
