/**Entidad para la consulta de datos */
export class QueryDataModel<F, O> {
  /**Orden*/
  order?: O;
  /**Dirección del ordenamiento*/
  orderAsc?: boolean;
  /**Filtro a utilizar*/
  filter?: F;
  /**Desde qué registro se deben obtener los datos*/
  from: number;
  /**Cantidad de registros a obtener*/
  length: number;
}

/**Entidad para la respuesta a las consultas de las base de datos */
export class ResponseDataModel<D> {
  /**Identificador de la llamada*/
  draw: number;
  /**Cantidad de registros filtrados*/
  recordsCount: number;
  /**Datos devueltos*/
  data: D[];
}
