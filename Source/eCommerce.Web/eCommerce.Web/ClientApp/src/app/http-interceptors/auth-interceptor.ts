import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';

/**Interceptor para capturar los errores de permisos */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    return next.handle(request).pipe(
      catchError((error: any) => {
        if (error.status === 401)
          this.router.navigate(['logout']);
        return throwError(error);
      })
    );
  }
}
