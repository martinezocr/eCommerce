import { FileModel } from "./file.model";

/**Entidad para representar un prestamo*/
export class RequestModel {
  requestId: number;
  requestTypeId: number;
  countryId: number;
  city: string;
  companyName: string;
  address: string;
  contact: string;
  phone: string;
  /*email: string;*/
  rut: string;
  requiredCredit: number;
  currencyId: number;
  remarks: string;
  dateOrder: Date;
  dateResponse: Date;
  files: FileModel[];
}

/**Campos de la solicitud */
export enum RequestFields {
  requestId = 0,
  country = 1,
  rut = 2,
  companyName = 3,
  currency = 4,
  requiredCredit = 5,
  dateOrder = 6,
  dateResponse = 7,
  status = 8,
  enterprise = 9
}

/**Clase para definir los valores de los filtros */
export class RequestFilter {
  freeText: string;
}
