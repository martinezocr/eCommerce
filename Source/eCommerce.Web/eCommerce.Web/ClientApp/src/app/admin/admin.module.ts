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
import { ProductsComponent } from './products/products.component';

//Diálogos
import { UserDialog } from './users/user.dialog';
import { EmptyComponent } from './empty/empty.component'
import { ProductDialog } from './products/product.dialog';

@NgModule({
  declarations: [
    UsersComponent,
    UserDialog,
    LoginComponent,
    EmptyComponent,
    ProductsComponent,
    ProductDialog
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
    UserDialog,
    ProductDialog
  ]
})
export class AdminModule { }
