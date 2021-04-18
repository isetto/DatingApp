import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../models/Member';
import { PaginatedResult } from '../models/pagination';
import { UserParams } from '../models/userParams';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl
  members: Member[] = []

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>>{
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize)
    params = params.append('minAge', userParams.minAge.toString())
    params = params.append('maxAge', userParams.maxAge.toString())
    params = params.append('gender', userParams.gender)
    const url = `${this.baseUrl}users`

    return this.getPaginatedResult<Member[]>(url, params )
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams()
    params = params.append('pageNumber', pageNumber.toString())
    params = params.append('pageSize', pageSize.toString())

    return params
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

  private getPaginatedResult<T>( url: string, params: any ): Observable<PaginatedResult<T>> {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    //{observe: 'response', params} causes that we will get full response from server which we need to get params header back
    return this.http.get<T>( url, { observe: 'response', params } ).pipe(
      map( (response: any) => {
        paginatedResult.result = response.body;
        if ( response.headers.get( 'Pagination' ) !== null ) {
          paginatedResult.pagination = JSON.parse( response.headers.get( 'Pagination' ) );
        }
        return paginatedResult;
      } )
    );
  }
}
