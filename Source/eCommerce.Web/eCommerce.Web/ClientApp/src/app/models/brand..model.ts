/**Entidad para representar a la empresa*/
export class BrandModel {
  brandId: number;
  name: string;
}

/**Campos de la empresa*/
export enum BrandFields {
  brandId = 0,
  name = 1
}

/**Clase para definir los valores de los filtros */
export class BrandFilter {
  freeText: string;
}
