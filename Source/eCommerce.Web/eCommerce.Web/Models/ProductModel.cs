
namespace eCommerce.Web.Models
{
    /// <summary>
    /// entidad para representar un producto
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// identificador
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// identificador de la marca
        /// </summary>
        public int BrandId { get; set; }
        /// <summary>
        /// identificador de la categoría
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// título del producto
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// descripción del producto
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// pricio del producto
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// establece si el producto está en descuento
        /// </summary>
        public bool IsDiscount { get; set; }
        /// <summary>
        /// precio anterior del producto
        /// </summary>
        public decimal? OldPrice { get; set; }
        /// <summary>
        /// establece si el producto está activo
        /// </summary>
        public bool IsActive { get; set; }

        public ProductImageModel[] Images { get; set; }

    }
}
