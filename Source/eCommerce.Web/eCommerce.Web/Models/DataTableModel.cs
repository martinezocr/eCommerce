namespace eCommerce.Web.Models
{
    /// <summary>
    /// Entidad para la respuesta a las consultas de las base de datos
    /// </summary>
    public class DataTableModel
    {
        /// <summary>
        /// Cantidad de registros filtrados
        /// </summary>
        public int RecordsCount { get; set; }

        /// <summary>
        /// Datos a devolver
        /// </summary>
        public dynamic Data { get; set; }
    }
}
