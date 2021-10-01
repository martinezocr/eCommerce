/**Entidad para representar a la empresa*/
export class CategoryModel {
  categoryId: number;
  name: string;
}

/**Campos de la empresa*/
export enum CategoryFields {
  categoryId = 0,
  name = 1
}

/**Clase para definir los valores de los filtros */
export class CategoryFilter {
  freeText: string;
}
