import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from './services/user.service';

@Injectable({
  providedIn: 'root'
})
export class LoggedGuard implements CanActivate {

  constructor(
    private userService: UserService,
    private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    if (!this.userService.isLoggedIn()) {
      return this.userService.updateUserFromServer()
        .then(res => {
          if (!res) {
            let navigateTo = state.url.split("/")[1];
            this.router.navigate([`${navigateTo}/ingreso`], { queryParams: { returnUrl: state.url } });
          }
          return res;
        })
    }
    return true;
  }
}
