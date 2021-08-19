import { MatSnackBarConfig } from "@angular/material/snack-bar";
import { MatPaginatorDefaultOptions } from "@angular/material/paginator";
import { MatTooltipDefaultOptions } from "@angular/material/tooltip";

export class Settings {
  public static TITLE = 'eCommerce';

  /**Valores por defecto para los snackbar */
  public static SNACKBAR_DEFAULTS: MatSnackBarConfig = { duration: 3000 };

  /**Valores por defecto para el delay de los tooltips */
  public static TOOLTIP_DEFAULTS: MatTooltipDefaultOptions = { showDelay: 500, hideDelay: 100, touchendHideDelay: 500 };

  /**Valores por defecto del paginador */
  public static PAGINATOR_DEFAULTS: MatPaginatorDefaultOptions = { showFirstLastButtons: true, pageSizeOptions: [8, 20, 50] };

  public static MONTHS = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
  public static DATE_LOCALE = 'es-AR';
  public static REGEX_URL = /^(https?):\/\/(-\.)?([^\s\\/?\.#-%=]+\.?)+(\/[^\s]*)?$/;
  public static REGEX_NUMBER_NATURAL = /^[0-9]*$/;
  public static REGEX_NUMBER_TWO_DECIMALS = /^[0-9]+(.[0-9]{0,2})?$/;
  public static REGEX_NUMBER_EIGHT_DECIMALS = /^[0-9]+(.[0-9]{0,8})?$/;
  public static REGEX_PERCENT = Settings.REGEX_NUMBER_TWO_DECIMALS;
  public static INT32_MAX = 2147483648;
  public static INT16_MAX = 32768;

  /**Mensajes de error */
  public static ERROR_COMM = 'Ha ocurrido un error en la comunicación con el servidor';
  public static ERROR_SAVING = 'Ha ocurrido un error al grabar los datos';
  public static ERROR_IMPORT = 'Ha ocurrido un error en la importación de datos';
  public static CANT_DELETE = 'No se ha podido eliminar el registro, posiblemente porque se esté utilizando';

  /**Constantes de notificaciones de las actividades */
  public static NOTIFICACION_DAYS = 10;

  /**Raíz de las URL de los controladores*/
  public static ROOT_CONTROLLERS: string;
}
