import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../models/Member';
import { PaginatedResult } from '../models/pagination';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl
  members: Member[] = []
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number): Observable<PaginatedResult<Member[]>>{
    let params = new HttpParams()
    if(page !== null && itemsPerPage !== null){
      params = params.append('pageNumber', page.toString())
      params = params.append('pageSize', itemsPerPage.toString())
    }
    //{observe: 'response', params} causes that we will get full response from server which we need to get params header back
    return this.http.get<Member[]>(`${this.baseUrl}users`, {observe: 'response', params}).pipe(
      map(response=>{
        this.paginatedResult.result = response.body
        if(response.headers.get('Pagination') !== null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'))
        }
        return this.paginatedResult;
      })
    )
  }

  getMember(username: string): Observable<Member>{
    const member = this.members.find((user: Member) => user.username === username)
    if(member !== undefined) return of(member)
    return this.http.get<Member>(`${this.baseUrl}users/${username}`)
  }

  updateMember(member: Member): Observable<any>{
    return this.http.put(`${this.baseUrl}users`, member).pipe(
      map(()=>{
        const index = this.members.indexOf(member)
        this.members[index] = member
      })
    )
  }

  setMainPhoto(photoId: number){
    return this.http.put(`${this.baseUrl}users/set-main-photo/${photoId}`, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(`${this.baseUrl}users/delete-photo/${photoId}`)
  }
}
