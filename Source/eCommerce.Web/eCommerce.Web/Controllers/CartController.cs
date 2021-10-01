using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eCommerce.Web.Models;
using System.Threading.Tasks;
using System;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la administración de los datos de carrito de compras
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// guarda o actualiza la información de un carrito de compras
        /// </summary>
        /// <param name="cart">informaciòn del carrito</param>
        /// <returns>devuelve el carrito de compras creado o actualizado</returns>
        [HttpPut()]
        public async Task<CartModel> SaveCartAsync([FromBody] CartModel cart)
        {
            try
            {
                var cartId = await Data.Cart.SaveAsync(cart);
                return await Data.Cart.GetByIdAsync(cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Carrito -> {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// devuelve la información del carrito de compras
        /// </summary>
        /// <param name="cartId">identificador delc arrito</param>
        /// <returns></returns>
        [HttpGet("{cartId}")]
        public async Task<CartModel> GetAsync(Guid cartId)
        {
            return await Data.Cart.GetByIdAsync(cartId);
        }

        /// <summary>
        /// eliimina el carrito de compras
        /// </summary>
        /// <param name="cartId">identificador del carrito</param>
        /// <returns></returns>
        [HttpDelete("{cartId:guid}")]
        public async Task<bool> DeleteCartAsync(Guid cartId)
        {
            return await Data.Cart.DeleteAsync(cartId);
        }
    }
}