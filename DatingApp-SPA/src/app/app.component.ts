import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/User';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  jwthelper = new JwtHelperService();
  constructor(private authservice: AuthService) {}
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authservice.decodedToken = this.jwthelper.decodeToken(token);
    }
    if (user) {
      this.authservice.loggedInUser = user;
      this.authservice.changeMemberPhoto(user.photoUrl);
    }
  }
}
