//Core de Angular
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

//Módulos
import { AdminRoutingModule } from './admin-routing.module';
import { MaterialModule } from '../material.module';
import { UtilsModule } from '../utils/utils.module'

//Componentes
import { UsersComponent } from './users/users.component';
import { LoginComponent } from './login/login.component';

//Diálogos
import { UserDialog } from './users/user.dialog';
import { EmptyComponent } from './empty/empty.component'

@NgModule({
  declarations: [
    UsersComponent,
    UserDialog,
    LoginComponent,
    EmptyComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    UtilsModule
  ],
  providers: [],
  entryComponents: [
    UserDialog
  ]
})
export class AdminModule { }
