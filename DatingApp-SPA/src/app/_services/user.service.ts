import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.baseUrl + 'user/';
  constructor(private http: HttpClient) {}

  getUsers(
    page?,
    itemsPerPage?,
    userParams?,
    likesparams?
  ): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<
      User[]
    >();
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }
    if (likesparams != null && likesparams === 'likers') {
      params = params.append('likers', 'true');
    } else if (likesparams != null && likesparams === 'likees') {
      params = params.append('likees', 'true');
    }
    return this.http
      .get<User[]>(this.baseUrl, { observe: 'response', params })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }
  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }
  updateUser(id: number, user: User) {
    return this.http.put<User>(this.baseUrl + id, user);
  }
  setMainPhoto(userId: number, id: number) {
    return this.http.post(
      this.baseUrl + userId + '/photos/' + id + '/setMain',
      {}
    );
  }
  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + userId + '/photos/' + id);
  }
  sendLike(userId: number, recipentId: number) {
    return this.http.post(this.baseUrl + userId + '/like/' + recipentId, {});
  }
}
