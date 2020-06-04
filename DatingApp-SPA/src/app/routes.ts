import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';

export const AppRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard]  },
  { path: 'lists', component: ListsComponent, canActivate: [AuthGuard]  },
  { path: 'messages', component: MessagesComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];


/*DUMMY ROUTE TO APPLY GUARDS FOR MULTIPLE ROUTS*/
// { path: 'home', component: HomeComponent },
//   {
//     path: '',
//     runGuardsAndResolvers: 'always',
//     canActivate: [AuthGuard],
//     children: [
//         { path: 'members', component: MemberListComponent },
//         { path: 'lists', component: ListsComponent },
//         { path: 'messages', component: MessagesComponent },
//     ]
//   },