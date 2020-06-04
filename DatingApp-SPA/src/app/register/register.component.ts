import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registerModel: any;
  registerForm = this.fb.group({
    username: [''],
    password: [''],
  });
  constructor(private fb: FormBuilder, private authService: AuthService
             ,private router: Router, private alertify: AlertifyService) {}

  ngOnInit() {}
  register() {
    this.registerModel = this.registerForm.value;
    this.authService.register(this.registerModel).subscribe( (next) => {
      this.alertify.success('register successful');
      this.registerForm.reset();
    }, (error) => {
      this.alertify.error(error);
    } );
  }
  cancel() {
    this.registerForm.reset();
    this.router.navigate(['/home']);
  }
  resetForm() {
    this.registerForm.reset();
  }
}
