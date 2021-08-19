import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services/user.service';
import { Settings } from '../app.settings';

@Component({
  selector: 'app-password-dialog',
  templateUrl: 'password.dialog.html',
  styleUrls: ['./password.dialog.scss']
})
export class PasswordDialog {

  passwordForm: FormGroup;
  hide = true;
  working = false;

  constructor(public dialogRef: MatDialogRef<PasswordDialog>,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private userService: UserService
  ) {

    //Creo el formulario vacío
    this.passwordForm = this.fb.group({
      current: ['', [Validators.required]],
      new: ['', [Validators.required, Validators.minLength(6)]],
      repeat: ['']
    }, { validator: this.checkPasswords });
  }

  /**
   * Valida que las contraseñas ingresadas sean igual
   * @param group Formulario que contiene las contraseñas
   */
  checkPasswords(group: FormGroup) {
    let pass = group.controls.new.value;
    let confirmPass = group.controls.repeat.value;
    return pass === confirmPass ? null : { notSame: true }
  }

  /**Realiza el cambio de la contraseña */
  changePassword(): void {
    if (this.passwordForm.valid)
      this.userService.changePassword(this.passwordForm.value)
        .then(res => {
          if (res) {
            this.snackBar.open('La contraseña ha sido cambiada');
            this.dialogRef.close();
          } else
            this.snackBar.open('La contraseña ingresada es incorrecta');
        })
        .catch(() => this.snackBar.open(Settings.ERROR_SAVING))
        .finally(() => this.working = false);
  }
}
