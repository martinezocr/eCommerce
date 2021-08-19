import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RequestFields, RequestFilter, RequestModel } from '../models/request.model';
import { ResponseDataModel, QueryDataModel } from '../models/api.model';
import { Settings } from '../app.settings';
import { FileModel } from '../models/file.model';

/**Clase para el manejo de los prestamos*/
@Injectable({
  providedIn: 'root'
})
export class RequestService {

  CONTROLLER: string;

  constructor(private http: HttpClient) {
    this.CONTROLLER = Settings.ROOT_CONTROLLERS + 'request/';
  }


  /**
   * Devuelve la lista de solicitudes
   * @param from Registro desde el cual obtener los datos
   * @param length Cantidad de registros a obtener
   * @param order Campo a utilizar para el orden
   * @param orderAsc Establece si el orden es ascendente
   * @param filter Filtros a utilizar
   * @param draw Identificador de la llamada
   */
  list(from: number, length: number, order: RequestFields, orderAsc: boolean, filter: RequestFilter, draw?: number): Promise<ResponseDataModel<RequestModel>> {
    const data = new QueryDataModel<RequestFilter, RequestFields>();

    data.filter = filter;
    data.from = from * length;
    data.length = length;
    data.order = order;
    data.orderAsc = orderAsc;

    return this.http.post<ResponseDataModel<RequestModel>>(this.CONTROLLER + 'list', data)
      .toPromise()
      .then(data => {
        data.draw = draw;
        return data;
      });
  }

  /**
   * Elimina una solicitud
   * @param requesId Identificador de la solicitud
   */
  delete(requesId: number): Promise<boolean> {
    return this.http.delete<boolean>(this.CONTROLLER + requesId)
      .toPromise();
  }

  /**
 * Devuelve los datos de una solicitud
 * @param requesId Identificador de de la solicitud
 */
  get(requesId: number): Promise<RequestModel> {
    return this.http.get<RequestModel>(this.CONTROLLER + requesId)
      .toPromise()
      .then(res => res as RequestModel);
  }

  /**
 * Graba los datos de una solicitud
 * @param data Datos a guardar
 */
  save(data: RequestModel, files: FileModel[]): Promise<boolean> {
    const form = new FormData();
    form.append('data', JSON.stringify(data));
    files.map(file => {
      form.append('files', file.file)
    });
    return this.http.put<boolean>(this.CONTROLLER, form)
      .toPromise();
  }

}
