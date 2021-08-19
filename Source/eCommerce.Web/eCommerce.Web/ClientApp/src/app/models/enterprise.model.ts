/**Entidad para representar a la empresa*/
export class EnterpriseModel {
  enterpriseId: number;
  name: string;
  address: string;
  email: string;
  contact: string;
}

/**Campos de la empresa*/
export enum EnterpriseFields {
  enterpriseId = 0,
  name = 1,
  address = 2,
  email = 3,
  contact = 4
}

/**Clase para definir los valores de los filtros */
export class EnterpriseFilter {
  freeText: string;
}
