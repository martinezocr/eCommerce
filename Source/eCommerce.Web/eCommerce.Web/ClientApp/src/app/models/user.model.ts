/**Entidad del usuario */
export class UserModel {
  userId: number;
  roles: UserRole[];
  username: string;
  isACtive: boolean;
  password: string;
  isActive: boolean;
  email: string;
  enterpriseId: number;
}

/**Campos del usuario */
export enum UserFields {
  userId = 0,
  username = 1,
  isActive = 2,
  loggedOn = 3
}

/**Clase para definir los valores de los filtros */
export class UserFilter {
  /**Filtro para el campo "Username"*/
  username: string;
  /**Filtro para el campo "IsActive"*/
  isActive: boolean;
  /**Búsqueda por texto libre*/
  freeText: string;
}

//Roles de los usuarios
export enum UserRole {
  Admin = 255,
  User = 1,
}

/**Datos del usuario logeado */
export class MyUserModel {
  /**Nombre de usuario*/
  username: string;
  /**Identificador del usuario*/
  userId: number;
  /**URL a utilizar en la home*/
  homeUrl: string;
  /**Roles del usuario*/
  roleIds: UserRole[];
}
