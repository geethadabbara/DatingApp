import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';

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
             ,private router: Router) {}

  ngOnInit() {}
  register() {
    this.registerModel = this.registerForm.value;
    this.authService.register(this.registerModel).subscribe( (next) => {
      console.log('register successful');
      this.registerForm.reset();
    }, (error) => {
      console.log( error);
    } );
  }
  cancel() {
    this.registerForm.reset();
    this.router.navigate(['/']);
  }
  resetForm() {
    this.registerForm.reset();
  }
}
