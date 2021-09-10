namespace eCommerce.Web
{
    /// <summary>
    /// Constantes de la aplicación
    /// </summary>
    public class Const
    {
        /// <summary>
        /// Ruta base para todos los controladores
        /// </summary>
        //internal const string ROOT_CONTROLLER = "api/{lang:regex(^(es|en|pt)$)}/[controller]";
        internal const string ROOT_CONTROLLER = "api/[controller]";

        /// <summary>
        /// Error del SQL por no tener permisos
        /// </summary>
        public const int SQL_NO_PERMISSION = -666;
        /// <summary>
        /// error de instegridad de sql al intentar borrar un registro
        /// </summary>
        public const int SQL_ERROR_DELETE = 547;
        /// <summary>
        /// path donde se van a guardar los archivos
        /// </summary>
        public const string FOLDER_FILE = "Files";
        /// <summary>
        /// tiempo de expiración del carrito de compras
        /// </summary>
        public const int CART_EXPIRATION_DAYS = 5;

    }
}
