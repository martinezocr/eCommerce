//Core de Angular
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//Diálogos
import { ConfirmDialog } from './dialogs/confirm.dialog';
import { PasswordDialog } from './dialogs/password.dialog';
import { AlertDialog } from './dialogs/alert.dialog';

//Módulos
import { AppRoutingModule } from './app-routing.module';
import { MaterialModule } from './material.module';

//Componentes
import { AppComponent } from './app.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AuthInterceptor } from './http-interceptors/auth-interceptor';
import { AdminComponent } from './admin/admin.component';

@NgModule({
  declarations: [
    AppComponent,
    NotFoundComponent,
    ConfirmDialog,
    PasswordDialog,
    AlertDialog,
    AdminComponent
  ],
  imports: [
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MaterialModule
  ],
  providers: [
    DecimalPipe,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }],
  bootstrap: [AppComponent],
  entryComponents: [
    ConfirmDialog,
    AlertDialog,
    PasswordDialog
  ]
})
export class AppModule { }
