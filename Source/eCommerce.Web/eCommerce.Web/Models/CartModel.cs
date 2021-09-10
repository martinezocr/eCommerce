using System;

namespace eCommerce.Web.Models
{
    /// <summary>
    /// Entidad para la representación del carrito de compras
    /// </summary>
    public class CartModel
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public Guid? CartId { get; set; }
        
        /// <summary>
        /// items del carrito
        /// </summary>
        public CartItemModel[] CartItems { get; set; }
    }

    /// <summary>
    /// entidad de los items del carrito
    /// </summary>
    public class CartItemModel
    {
        /// <summary>
        /// identificador del item
        /// </summary>
        public int? CartItemId { get; set; }
        /// <summary>
        /// identificador del carrito de compras
        /// </summary>
        public Guid CartId { get; set; }
        /// <summary>
        /// identificador del producto
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// cantidad de items seleccionados a comprar
        /// </summary>
        public byte Amount { get; set; }
        /// <summary>
        /// nombre/titulo del producto
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Descripción del producto
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// precio del producto
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// orden en que se va a mostrar el producto en el carrito
        /// </summary>
        public byte Order { get; set; }
    }
}