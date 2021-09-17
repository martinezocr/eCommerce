import { Inject, Injectable, LOCALE_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserModel, UserFilter, UserFields, MyUserModel, UserRole } from '../models/user.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';

/**Clase para el manejo de los usuarios */
@Injectable({
  providedIn: 'root'
})
export class UserService {
  API: string;
  public myUser: MyUserModel = null;
  public myName: string = null;

  constructor(
    @Inject(LOCALE_ID) public locale: string,
    private http: HttpClient
  ) {
    this.API = Settings.ROOT_CONTROLLERS + 'user/';
  }

  /**
   * Devuelve la lista de usuarios
   * @param from Registro desde el cual obtener los datos
   * @param length Cantidad de registros a obtener
   * @param order Campo a utilizar para el orden
   * @param orderAsc Establece si el orden es ascendente
   * @param filter Filtros a utilizar
   * @param draw Identificador de la llamada
   */
  list(from: number, length: number, order: UserFields, orderAsc: boolean, filter: UserFilter, draw?: number): Promise<ResponseDataModel<UserModel>> {
    const data = new QueryDataModel<UserFilter, UserFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<UserModel>>(this.API + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
   * Devuelve la lista de usuarios
   */
  listAll(): Promise<UserModel[]> {
    return this.http.get<UserModel[]>(this.API + 'all')
      .toPromise();
  }

  /**
   * Elimina un usuario
   * @param userId Identificador del usuario
   */
  delete(userId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.API + userId)
      .toPromise();
  }

  /**
   * Autentica al usuario
   * @param username Nombre del usuario
   * @param password Contraseña del usuario
   */
  auth(username: string, password: string, remember: boolean) {
    return this.http.post<MyUserModel>(this.API + 'auth', { username: username, password: password, remember: remember })
      .toPromise()
      .then(res => {
        if (res === null)
          return false;
        this.myUser = res;
        this.myName = this.myUser.firstName;
        sessionStorage.setItem('user', JSON.stringify(res));
        return true;
      });
  }

  /**
   * Autentica a un cliente
   * @param tournamentId Torneo
   * @param colorId Color
   * @param password Contraseña
   */
  //authCompetitor(tournamentId: number, colorId: number, password: string) {
  //  return this.http.post<MyUserModel>(Settings.ROOT_CONTROLLERS + 'competitor/auth', { tournamentId: tournamentId, colorId: colorId, password: password })
  //    .toPromise()
  //    .then(res => {
  //      if (res === null)
  //        return false;
  //      this.myUser = res;
  //      this.myName = null;
  //      sessionStorage.setItem('user', JSON.stringify(res));
  //      //this.signalRService.open();
  //      return true;
  //    });
  //}

  /**
   * Verifica si el usuario está logeado
   */
  updateUser() {
    const json = sessionStorage.getItem('user');
    if (json !== null) {
      this.myUser = JSON.parse(json);
      if (this.myUser !== null && this.myUser.userId !== null || this.myUser.userId !== 0)
        this.myName = this.myUser.firstName;
    }
  }

  /**
   * Verifica si el usuario está logeado pidiendo los datos al servidor
   */
  updateUserFromServer(): Promise<boolean> {
    return this.http.get<MyUserModel>(Settings.ROOT_CONTROLLERS + 'user')
      .toPromise()
      .then(res => {
        if (res === null || ((res.userId === null || res.userId === 0))) {
          this.myUser = null;
          this.myName = null;
          sessionStorage.removeItem('user');
          return false;
        }
        this.myUser = res;
        this.myName = this.myUser.firstName;
        sessionStorage.setItem('user', JSON.stringify(res));
        return true;
      });
  }

  /**Deslogea al usuario */
  logout(): Promise<null> {
    sessionStorage.removeItem('user');
    this.myUser = null;
    this.myName = null;
    //this.signalRService.close();
    return this.http.get<null>(this.API + 'logout').toPromise();
  }

  /**
   * Devuelve si un usuario posee uno de los roles especificados
   * @param roles Roles a buscar
   */
  hasRole(roles: UserRole[]): boolean {
    if (this.myUser === null || this.myUser.roleIds === null)
      return false;
    for (const role of roles)
      if (this.myUser.roleIds.indexOf(role) >= 0)
        return true;
    return false;
  }

  /**Devuelve si un usuario posee algún rol */
  hasSomeRole(): boolean {
    return this.myUser !== null && this.myUser.roleIds !== null && this.myUser.roleIds.length > 0;
  }

  /**Devuelve si el usuario está logeado */
  isLoggedIn(): boolean {
    if (this.myUser !== null)
      return true;
    this.updateUser();
    return this.myUser !== null;
  }

  /**
   * Devuelve los datos de un usuario
   * @param userId Identificador del usuario
   */
  get(userId: number): Promise<UserModel> {
    return this.http.get<UserModel>(this.API + userId)
      .toPromise()
      .then(res => res as UserModel);
  }

  /**
   * Graba los datos de un usuario
   * @param data Datos a guardar
   */
  save(data: UserModel): Promise<boolean> {
    return this.http.put<boolean>(this.API, data)
      .toPromise();
  }

  /**
   * Cambia la contraseña del usuario
   * @param data Contraseña actual y nueva
   */
  changePassword(data: any): Promise<null> {
    return this.http.post<null>(this.API + 'password', data)
      .toPromise();
  }
}
