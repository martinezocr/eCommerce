/**Entidad del usuario */
export class UserModel {
  userId: number;
  roles: UserRole[];
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
}

/**Campos del usuario */
export enum UserFields {
  userId = 0,
  username = 1,
  firstName = 2,
  lastName = 3,
  client = 4,
  isActive = 5,
  loggedOn = 6
}

/**Clase para definir los valores de los filtros */
export class UserFilter {
  /**Filtro para el campo "Username"*/
  username: string;
  /**Filtro para el campo "FirstName"*/
  firstName: string;
  /**Filtro para el campo "LastName"*/
  lastName: string;
  /**Filtro para el campo "IsActive"*/
  isActive: boolean;
  /**Búsqueda por texto libre*/
  freeText: string;
}

//Roles de los usuarios
export enum UserRole {
  //Competidor
  Competitor = 1,
  //Administrador global
  Admin = 255,
  //accesso a reportes
  AccessReports = 254
}

/**Datos del usuario logeado */
export class MyUserModel {
  /**Nombre de usuario*/
  username: string;
  /**Identificador del usuario*/
  userId: number;
  /**Identificador del equipo*/
  competitorId: number;
  /**Color del equipo*/
  competitorColor: ColorModel;
  /**Nombre del usuario*/
  name: string;
  /**Nombre del usuario*/
  firstName: string;
  /**URL a utilizar en la home*/
  homeUrl: string;
  /**Roles del usuario*/
  roleIds: UserRole[];
}
