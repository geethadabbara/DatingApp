import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/User';
import { Router, ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/Pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  userParams: any = {};
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];
  pagination: Pagination;
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });
    this.userParams.gender = this.user.gender === 'male' ? 'female' : 'male';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 90;
    this.userParams.orderBy = 'lastActive';
  }

  resetFilters() {
    this.userParams.gender = this.user.gender === 'male' ? 'female' : 'male';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 90;
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPageIndex = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPageIndex,
        this.pagination.itemsPerPage,
        this.userParams
      )
      .subscribe(
        (response: PaginatedResult<User[]>) => {
          console.log('users::', response);
          this.users = response.result;
          this.pagination = response.pagination;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}