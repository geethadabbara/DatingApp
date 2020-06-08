import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { User } from '../_models/User';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResovler implements Resolve<User> {
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<User> {
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
      catchError((error) => {
        this.alertify.error('problem retrieving the data');
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
  /**
   *
   */
  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService,
    private authService: AuthService
  ) {}
}
