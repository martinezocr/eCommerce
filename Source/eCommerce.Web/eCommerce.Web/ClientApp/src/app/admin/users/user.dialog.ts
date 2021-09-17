import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms'
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../services/user.service';
import { Settings } from '../../app.settings';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-user',
  templateUrl: './user.dialog.html',
  styleUrls: ['./user.dialog.scss']
})
export class UserDialog implements OnInit {
  working = true;
  userForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<UserDialog>,
    @Inject(MAT_DIALOG_DATA) private userId: number,
    private userService: UserService,
    private snackBar: MatSnackBar) {
    //Creo el formulario
    this.userForm = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      isActive: true,
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      roles: null,
      password: [null, userId === null || userId === 0 ? [Validators.required, Validators.minLength(6)] : [Validators.minLength(6)]],
      confirmPassword: [null, userId === null || userId === 0 ? [Validators.required] : null]
    }, { validator: this.checkPasswords });
  }

  checkPasswords(group: FormGroup) { // here we have the 'passwords' group
    return group.controls.password.value === group.controls.confirmPassword.value ? null : { notSame: true }
  }

  ngOnInit() {
    //Cargo los datos del usuario
    if (this.userId !== undefined && this.userId !== null)
      this.userService.get(this.userId)
        .then(data => this.userForm.patchValue(data))
        .catch(() => {
          this.snackBar.open(Settings.ERROR_COMM);
          this.dialogRef.close(false);
        })
        .finally(() => this.working = false);
    else
      this.working = false;
  }

  /**Cancela la edición */
  cancel() {
    this.dialogRef.close(false);
  }

  /**Grabación de los datos */
  save(): void {
    if (this.userForm.valid) {
      const model = this.userForm.value as UserModel;
      model.userId = this.userId;

      this.working = true;
      this.userService.save(model)
        .then((res) => {
          if (res)
            this.dialogRef.close(true)
          else
            this.snackBar.open('Ese nombre de usuario ya está siendo utilizado');
        })
        .catch(() => this.snackBar.open(Settings.ERROR_SAVING))
        .finally(() => this.working = false);
    }
  }
}
