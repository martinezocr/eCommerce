using System;

namespace eCommerce.Web.Models
{
    /// <summary>
    /// entidad de la categoría
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// identificador de la categoría
        /// </summary>
        public int? CategoryId { get; set; }
        /// <summary>
        /// nombre/descripción
        /// </summary>
        public string Name { get; set; }
    }
}
