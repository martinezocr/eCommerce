import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnterpriseModel, EnterpriseFilter, EnterpriseFields } from '../models/enterprise.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';


/**Clase para el manejo de los clientes */
@Injectable({
  providedIn: 'root'
})
export class EnterpriseService {
  API: string;
  constructor(private http: HttpClient) {
    this.API = Settings.ROOT_CONTROLLERS + 'enterprise/';
  }

  /**
   * Devuelve la lista de las empresas
   * @param from Registro desde el cual obtener los datos
   * @param length Cantidad de registros a obtener
   * @param order Campo a utilizar para el orden
   * @param orderAsc Establece si el orden es ascendente
   * @param filter Filtros a utilizar
   * @param draw Identificador de la llamada
   */
  list(from: number, length: number, order: EnterpriseFields, orderAsc: boolean, filter: EnterpriseFilter, draw?: number): Promise<ResponseDataModel<EnterpriseModel>> {
    const data = new QueryDataModel<EnterpriseFilter, EnterpriseFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<EnterpriseModel>>(this.API + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
  * Devuelve la lista de empresas
  */
  listAll(): Promise<EnterpriseModel[]> {
    return this.http.get<EnterpriseModel[]>(this.API + 'all')
      .toPromise();
  }

  /**
   * Elimina un cliente
   * @param userId Identificador del cliente
   */
  delete(clientId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.API + clientId)
      .toPromise();
  }

  /**
 * Devuelve los datos de un cliente
 * @param enterpriseId Identificador de la empresa
 */
  get(enterpriseId: number): Promise<EnterpriseModel> {
    return this.http.get<EnterpriseModel>(this.API + enterpriseId)
      .toPromise()
      .then(res => res as EnterpriseModel);
  }

  /**
 * Graba los datos de una empresa
 * @param data Datos a guardar
 */
  save(data: EnterpriseModel): Promise<boolean> {
    return this.http.put<boolean>(this.API, data)
      .toPromise();
  }

}
