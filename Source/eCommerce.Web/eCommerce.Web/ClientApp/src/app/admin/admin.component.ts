import { Component, Inject, LOCALE_ID, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Settings } from '../app.settings';
import { UserService } from '../services/user.service';
import { PasswordDialog } from '../dialogs/password.dialog';
import { UserRole } from '../models/user.model';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent {
  UserRole = UserRole;
  title = Settings.TITLE;

  constructor(
    @Inject(LOCALE_ID) public locale: string,
    private titleService: Title,
    private dialog: MatDialog,
    public userService: UserService,
    private snackBar: MatSnackBar,
    public router: Router) {

    this.titleService.setTitle(Settings.TITLE);
    this.userService.updateUser();
    this.userService.updateUserFromServer()
      .then(ok => {
        if (!ok)
          this.router.navigate(['/admin/ingreso']);
      });
  }

  /**Cambio de contraseña */
  changePassword(): void {
    this.dialog.open(PasswordDialog, {
      autoFocus: true
    });
  }

  /**Deslogeo del usuario */
  logout(): void {
    this.userService.logout()
      .then(() => this.router.navigate(['/admin/ingreso']))
      .catch(() => this.snackBar.open(Settings.ERROR_COMM));
  }
}
