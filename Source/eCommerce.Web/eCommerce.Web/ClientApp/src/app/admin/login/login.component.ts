import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../services/user.service';
import { Settings } from '../../app.settings';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  returnUrl: string;
  loginForm: FormGroup;
  showLogin = false;
  @ViewChild('rememberMe') rememberMe: MatSlideToggle;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router) {

    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/administrador';

    if (!this.userService.isLoggedIn())
      this.userService.updateUserFromServer().then(res => {
        if (res)
          this.router.navigateByUrl(this.returnUrl);
        else
          this.showLogin = true;
      })
        .catch(() => this.snackBar.open(Settings.ERROR_COMM))
        .finally(() => this.loginForm.enable());
    else
      this.router.navigate(['/administrador']);
  }

  auth(): void {
    if (!this.loginForm.valid)
      return;
    this.loginForm.disable();
    this.userService.auth(this.loginForm.controls.username.value, this.loginForm.controls.password.value, this.rememberMe.checked)
      .then(res => {
        if (res)
          this.router.navigateByUrl(this.returnUrl);
        else
          this.snackBar.open(`El usuario o la contraseña ingresados son incorrectos`);
      })
      .catch(() => this.snackBar.open(Settings.ERROR_COMM))
      .finally(() => this.loginForm.enable());
  }
}
