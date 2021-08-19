import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from './services/user.service';

/**Guard para verificar que el usuario tenga al menos uno de los roles requeridos para ejecutar el componente */
@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  constructor(
    private userService: UserService,
    private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (!this.userService.isLoggedIn()) {
      let navigateTo = state.url.split("/")[1];
      this.router.navigate([`${navigateTo}/ingreso`], { queryParams: { returnUrl: state.url } });
      return false;
    }
    if (next.data['roles'] === null || next.data['roles'].length === 0)
      return true;
    return this.userService.hasRole(next.data['roles']);
  }
}
