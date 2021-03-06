import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResovler } from './_resolvers/member-detail.resolver';
import { MemberListResovler } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResovler } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { ListsResovler } from './_resolvers/lists.resolver';
import { MessagesResovler } from './_resolvers/messages.resolver';

export const AppRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'members',
    component: MemberListComponent,
    resolve: {
      users: MemberListResovler,
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'members/:id',
    component: MemberDetailComponent,
    resolve: {
      user: MemberDetailResovler,
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'member/edit',
    component: MemberEditComponent,
    canDeactivate: [PreventUnsavedChangesGuard],
    resolve: {
      user: MemberEditResovler,
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'lists',
    component: ListsComponent,
    canActivate: [AuthGuard],
    resolve: {
      users: ListsResovler,
    },
  },
  {
    path: 'messages',
    component: MessagesComponent,
    canActivate: [AuthGuard],
    resolve: {
      messages: MessagesResovler,
    },
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

/*DUMMY ROUTE TO APPLY GUARDS FOR MULTIPLE ROUTS*/
// { path: '', component: HomeComponent },
// {
//     path: '',
//     runGuardsAndResolvers: 'always',
//     canActivate: [AuthGuard],
//     children: [
//         { path: 'members', component: MemberListComponent },
//         { path: 'lists', component: ListsComponent },
//         { path: 'messages', component: MessagesComponent },
//     ]
// },
// { path: 'register', component: RegisterComponent },
// { path: '**', redirectTo: '', pathMatch: 'full' }
