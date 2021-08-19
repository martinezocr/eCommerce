import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserModel, UserFilter, UserFields, MyUserModel, UserRole } from '../models/user.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';

export enum SaveResult {
  /**todo bien */
  ok = 0,
  /**la pregunta requiere al menos dos respuestas */
  usernameExists = -1,
}

export enum ValidateResult {
  /**todo bien */
  ok = 1,
  /**el usuario no existe */
  notExists = 0,
  /**el usuario esta bloqueado */
  isLocked = -1,
}


/**Clase para el manejo de los usuarios */
@Injectable({
  providedIn: 'root'
})
export class UserService {

  public myUser: MyUserModel = null;
  /*public myName: string = null;*/

  constructor(private http: HttpClient) {
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

    return this.http.post<ResponseDataModel<UserModel>>('/api/user/list', data)
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
    return this.http.get<UserModel[]>('/api/user/all')
      .toPromise();
  }

  /**
   * Elimina un usuario
   * @param userId Identificador del usuario
   */
  delete(userId: number): Promise<boolean> {
    return this.http.delete<boolean>('/api/user/' + userId)
      .toPromise();
  }

  /**
   * Autentica al usuario
   * @param username Nombre del usuario
   * @param password Contraseña del usuario
   */
  auth(username: string, password: string, remember: boolean): Promise<ValidateResult> {
    return this.http.post<any>('/api/user/auth', { username: username, password: password, remember: remember })
      .toPromise()
      .then(res => {
        if (res === null)
          return ValidateResult.notExists;
        if (res.isLocked == true)
          return ValidateResult.isLocked;
        this.myUser = res;
        //this.myName = this.myUser.firstName;
        sessionStorage.setItem('user', JSON.stringify(res));
        return ValidateResult.ok;
      });
  }

  /**
  * Verifica si el usuario está logeado
  */
  updateUser() {
    const json = sessionStorage.getItem('user');
    if (json !== null) {
      this.myUser = JSON.parse(json);
      //if (this.myUser !== null && this.myUser.userId !== null || this.myUser.userId !== 0)
      //this.myName = this.myUser.firstName;
    }
  }

  /**
  * Verifica si el usuario está logeado pidiendo los datos al servidor
  */
  updateUserFromServer(): Promise<boolean> {
    return this.http.get<any>('/api/user')
      .toPromise()
      .then(res => {
        if (res === null || res.userId === null || res.userId === 0) {
          this.myUser = null;
          //this.myName = null;
          return false;
        }
        this.myUser = res;
        //this.myName = this.myUser.firstName;
        sessionStorage.setItem('user', JSON.stringify(res));
        return true;
      });
  }

  /**Deslogea al usuario */
  logout(): Promise<null> {
    sessionStorage.removeItem('user');
    this.myUser = null;
    //this.myName = null;
    return this.http.get<null>('/api/user/logout').toPromise();
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
    return this.http.get<UserModel>('/api/user/' + userId)
      .toPromise()
      .then(res => res as UserModel);
  }

  /**
 * Graba los datos de un usuario
 * @param data Datos a guardar
 */
  save(data: UserModel): Promise<boolean> {
    return this.http.put<boolean>('/api/user', data)
      .toPromise();
  }

  /**
   * Cambia la contraseña del usuario
   * @param data Contraseña actual y nueva
   */
  changePassword(data: any): Promise<null> {
    return this.http.post<null>('/api/user/password', data)
      .toPromise();
  }

}
