import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';

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
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  constructor(private http :HttpClient) { }

  getMembers(page?: number, itmesPerPage?: number){
    //if(this.getMembers.length > 0)return of(this.members);
    let params = new HttpParams();

    if(page != null && itmesPerPage != null){
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itmesPerPage.toString());
    }

    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
      map(response =>{
        this.paginatedResult.result = response.body;
        if(response.headers.get('Pagination') !== null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') || '{}');
        }
        return this.paginatedResult;
      })
     
      // map(members =>{
      //   this.members = members;
      //   return members;
      // })
    );
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
