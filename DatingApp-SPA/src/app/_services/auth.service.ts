import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.baseUrl + 'auth/';
  jwthelper = new JwtHelperService();
  decodedToken: any = {};
  loggedInUser: any = {};
  photoUrl: BehaviorSubject<string> = new BehaviorSubject<string>(
    '../../assets/images/user.png'
  );
  currentPhotoUrl = this.photoUrl.asObservable();
  constructor(private http: HttpClient) {}
  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwthelper.decodeToken(user.token);
          this.loggedInUser = user.user;
          this.changeMemberPhoto(this.loggedInUser.photoUrl);
        }
      }),
      catchError((error) => {
        return throwError(error);
      })
    );
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwthelper.isTokenExpired(token);
  }
}
