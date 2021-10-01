import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BrandFields, BrandFilter, BrandModel } from '../models/brand..model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';

/**Clase para el manejo de los prestamos*/
@Injectable({
  providedIn: 'root'
})
export class BrandService {

  API: string;

  constructor(private http: HttpClient) {
    this.API = Settings.ROOT_CONTROLLERS + 'brand/';
  }

  /**devuelve la lista de categorías */
  listAll(): Promise<BrandModel[]> {
    return this.http.get<BrandModel[]>(this.API + 'list-all')
      .toPromise()
      .then(res => res as BrandModel[]);
  }

  /**
   * Devuelve la lista de categoría
   * @param from Registro desde el cual obtener los datos
   * @param length Cantidad de registros a obtener
   * @param order Campo a utilizar para el orden
   * @param orderAsc Establece si el orden es ascendente
   * @param filter Filtros a utilizar
   * @param draw Identificador de la llamada
   */
  list(from: number, length: number, order: BrandFields, orderAsc: boolean, filter: BrandFilter, draw?: number): Promise<ResponseDataModel<BrandModel>> {
    const data = new QueryDataModel<BrandFilter, BrandFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<BrandModel>>(this.API + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
   * Elimina una marca
   * @param brandId Identificador de la marca
   */
  delete(brandId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.API + brandId)
      .toPromise();
  }

  /**
 * Devuelve los datos de una marca
 * @param brandId Identificador de de la marca
 */
  get(brandId: number): Promise<BrandModel> {
    return this.http.get<BrandModel>(this.API + brandId)
      .toPromise()
      .then(res => res as BrandModel);
  }

  /**
   * Graba los datos de una marca
   * @param data Datos a guardar
   */
  save(data: BrandModel): Promise<boolean> {
    return this.http.put<boolean>(this.API, data)
      .toPromise();
  }
}
