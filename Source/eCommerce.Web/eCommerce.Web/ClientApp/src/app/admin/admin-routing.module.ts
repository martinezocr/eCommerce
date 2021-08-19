import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserRole } from '../models/user.model';
import { RoleGuard } from '../role.guard';
import { UsersComponent } from './users/users.component';
import { LoginComponent } from './login/login.component';
import { EmptyComponent } from './empty/empty.component';

const routes: Routes = [
  {
    path: '',
    component: EmptyComponent,
    data: { roles: [UserRole.Admin], login: '/administrador/ingreso' },
    canActivate: [RoleGuard]
  },
  {
    path: 'usuarios',
    component: UsersComponent,
    data: { roles: [UserRole.Admin], login: '/administrador/ingreso' },
    canActivate: [RoleGuard]
  },
  //{
  //  path: 'torneos',
  //  loadChildren: () => import('../crud/tournaments/tournaments.module').then(mod => mod.TournamentsModule),
  //  data: { roles: [UserRole.Admin], login: '/admin/ingreso' },
  //  canActivate: [RoleGuard]
  //},

  { path: 'ingreso', component: LoginComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
