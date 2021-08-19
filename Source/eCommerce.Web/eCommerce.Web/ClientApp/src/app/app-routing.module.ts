import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './not-found/not-found.component';
import { StoreComponent } from './store/store/store.component';

const routes: Routes = [
  //{
  //  path: '',
  //  redirectTo: '/home',
  //  pathMatch: 'full'
  //},
  {
    path: '',
    component: StoreComponent,
    loadChildren: () => import('./store/store.module').then(mod => mod.StoreModule),
  },
  {
    path: 'administrador',
    loadChildren: () => import('./admin/admin.module').then(mod => mod.AdminModule),
  },
  //{
  //  path: 'establecimientos',
  //  loadChildren: () => import('./establishments/establishments.module').then(mod => mod.EstablishmentsModule),
  //  data: { roles: [UserRole.Admin] },
  //  canActivate: [RoleGuard]
  //},
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
