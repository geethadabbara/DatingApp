import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { tokenName } from '@angular/compiler';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  loginModel: any = {};
  loginForm = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
  });

  constructor(private fb: FormBuilder, private authService: AuthService) {}

  ngOnInit() {}

  login() {
    this.loginModel = this.loginForm.value;
    console.log(this.loginModel);
    this.authService.login(this.loginModel).subscribe(
      (next) => {
        console.log('logged in successfully');
      },
      (error) => {
        console.log('Failed to login' + error);
      }
    );
  }
  loggedIn(){
    const token = localStorage.getItem('token');
    return !!token;
  }
  logout() {
    localStorage.removeItem('token');
    console.log('user logged out');
  }
}
