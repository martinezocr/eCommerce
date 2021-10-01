using System;

namespace eCommerce.Web.Models
{
    /// <summary>
    /// entidad de la marca
    /// </summary>
    public class BrandModel
    {
        /// <summary>
        /// identificador de la marca
        /// </summary>
        public int? BrandId { get; set; }
        /// <summary>
        /// nombre/descripción
        /// </summary>
        public string Name { get; set; }
    }
}
