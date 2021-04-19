import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../models/Member';
import { PaginatedResult } from '../models/pagination';
import { User } from '../models/User';
import { UserParams } from '../models/userParams';
import { AccountService } from './account.service';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl
  members: Member[] = []
  memberCache = new Map()
  user: User
  userParams: UserParams

  constructor(private http: HttpClient, accountService: AccountService) { 
    accountService.currentUser$.pipe(take(1)).subscribe((user: User)=> {
      this.user = user
      this.userParams = new UserParams(user)
    })
  }

  getUserParams(){
    return this.userParams
  }

  setUserParams(params: UserParams){
    this.userParams = params
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user)
    return this.userParams
  }

  getMembers(userParams: UserParams): Observable<PaginatedResult<Member[]>>{
    var cachedResponse = this.memberCache.get(Object.values(userParams).join('-')) //get cached response
    if(cachedResponse){ //if exists then return it, dont make http call
      return of(cachedResponse)
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize)
    params = params.append('minAge', userParams.minAge.toString())
    params = params.append('maxAge', userParams.maxAge.toString())
    params = params.append('gender', userParams.gender)
    params = params.append('orderBy', userParams.orderBy)
    const url = `${this.baseUrl}users`

    return this.getPaginatedResult<Member[]>(url, params ).pipe(
      map(response=>{
        this.memberCache.set(Object.values(userParams).join('-'), response) //set cached response
        return response
      })
    )
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams()
    params = params.append('pageNumber', pageNumber.toString())
    params = params.append('pageSize', pageSize.toString())

    return params
  }

  getMember(username: string): Observable<Member>{
    const member = [...this.memberCache.values()]
    .reduce((arr: Member[], elem) => arr.concat(elem.result), [])
    .find((member: Member)=> member.username === username)
    if(member) return of(member)
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
