import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../models/pagination";

export function getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams()
    params = params.append('pageNumber', pageNumber.toString())
    params = params.append('pageSize', pageSize.toString())

    return params
  }

  export function getPaginatedResult<T>( url: string, params: any, http: HttpClient): Observable<PaginatedResult<T>> {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    //{observe: 'response', params} causes that we will get full response from server which we need to get params header back
    return http.get<T>( url, { observe: 'response', params } ).pipe(
      map( (response: any) => {
        paginatedResult.result = response.body;
        if ( response.headers.get( 'Pagination' ) !== null ) {
          paginatedResult.pagination = JSON.parse( response.headers.get( 'Pagination' ) );
        }
        return paginatedResult;
      } )
    );
  }