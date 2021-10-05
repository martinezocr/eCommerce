
using System;
using System.Collections.Generic;

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

    class ProductModelComparer : IEqualityComparer<ProductModel>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(ProductModel x, ProductModel y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.ProductId == y.ProductId;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(ProductModel product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Code field.
            int hashProductCode = product.ProductId.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductCode;
        }
    }
}
