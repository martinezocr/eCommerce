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
                return null;
            }
        }

        /// <summary>
        /// genera la descargar del excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("{cartId:guid}")]
        public async Task<CartModel> GetAsync(Guid cartId)
        {
            return await Data.Cart.GetByIdAsync(cartId);
        }

        [HttpDelete("{cartId:guid}")]
        public async Task<bool> DeleteCartAsync(Guid cartId)
        {
            return await Data.Cart.DeleteAsync(cartId);
        }
    }
}