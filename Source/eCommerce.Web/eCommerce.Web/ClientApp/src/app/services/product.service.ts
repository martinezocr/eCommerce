import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductModel, ProductFilter, ProductFields, ProductImageModel } from '../models/product.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';

export enum SaveResult {
  /**todo bien */
  ok = 0,
  /**no contiene imágenes */
  cantImages = -1,
  /**Precio anterior requerido cuando hay un descuento */
  OldPriceRequiredForDiscount = -2
}


/**Clase para el manejo de los productos */
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  API: string;
  constructor(private http: HttpClient) {
    this.API = Settings.ROOT_CONTROLLERS + 'product/';
  }

  /**
   * Devuelve la lista de productos
   * @param from Registro desde el cual obtener los datos
   * @param length Cantidad de registros a obtener
   * @param order Campo a utilizar para el orden
   * @param orderAsc Establece si el orden es ascendente
   * @param filter Filtros a utilizar
   * @param draw Identificador de la llamada
   */
  list(from: number, length: number, order: ProductFields, orderAsc: boolean, filter: ProductFilter, draw?: number): Promise<ResponseDataModel<ProductModel>> {
    const data = new QueryDataModel<ProductFilter, ProductFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<ProductModel>>(this.API + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
  * Devuelve la lista de productos
  */
  listAll(): Promise<ProductModel[]> {
    return this.http.get<ProductModel[]>(this.API + 'all')
      .toPromise();
  }

  /**
   * Elimina un producto
   * @param userId Identificador del cliente
   */
  delete(productId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.API + productId)
      .toPromise();
  }

  /**
 * Devuelve los datos de un cliente
 * @param productId Identificador del producto
 */
  get(productId: number): Promise<ProductModel> {
    return this.http.get<ProductModel>(this.API + productId)
      .toPromise()
      .then(res => res as ProductModel);
  }

  /**
     * Graba los datos de una pregunta
     * @param data Datos a guardar
     */
  save(data: ProductModel, files: ProductImageModel[]): Promise<SaveResult> {
    const form = new FormData();
    form.append('data', JSON.stringify(data));
    files.map(file => {
      form.append('files', file.file)
    });
    return this.http.put<SaveResult>(this.API, form)
      .toPromise();
  }
}
