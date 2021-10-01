import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CategoryFields, CategoryFilter, CategoryModel } from '../models/category.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';

/**Clase para el manejo de los prestamos*/
@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  API: string;

  constructor(private http: HttpClient) {
    this.API = Settings.ROOT_CONTROLLERS + 'category/';
  }

  /**devuelve la lista de categorías */
  listAll(): Promise<CategoryModel[]> {
    return this.http.get<CategoryModel[]>(this.API + 'list-all')
      .toPromise()
      .then(res => res as CategoryModel[]);
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
  list(from: number, length: number, order: CategoryFields, orderAsc: boolean, filter: CategoryFilter, draw?: number): Promise<ResponseDataModel<CategoryModel>> {
    const data = new QueryDataModel<CategoryFilter, CategoryFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<CategoryModel>>(this.API + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
   * Elimina una categoría
   * @param categoryId Identificador de la categoría
   */
  delete(categoryId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.API + categoryId)
      .toPromise();
  }

  /**
 * Devuelve los datos de una categoría
 * @param categoryId Identificador de de la categoría
 */
  get(categoryId: number): Promise<CategoryModel> {
    return this.http.get<CategoryModel>(this.API + categoryId)
      .toPromise()
      .then(res => res as CategoryModel);
  }

  /**
   * Graba los datos de una categoría
   * @param data Datos a guardar
   */
  save(data: CategoryModel): Promise<boolean> {
    return this.http.put<boolean>(this.API, data)
      .toPromise();
  }
}
