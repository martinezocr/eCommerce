using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eCommerce.Web.Models;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la administración de los datos de los usuarios
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
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

        //[HttpPut()]
        //public async Task<CartModel> SaveCartAsync([FromBody] CartModel cart)
        //{
        //    // Basket will be available for 15 days in memory
        //    var created = await _database.StringSetAsync(cart.CartId, JsonConvert.SerializeObject(cart), TimeSpan.FromMinutes(10));
        //    if (!created) 
        //        return null;

        //    return await GetAsync(cart.CartId);
        //}

        ///// <summary>
        ///// genera la descargar del excel
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("{cartId}")]
        //public async Task<CartModel> GetAsync(string cartId)
        //{
        //    var cart = await _database.StringGetAsync(cartId);
        //    return cart.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CartModel>(cart);
        //}

        //[HttpDelete("{cartId}")]
        //public async Task<bool> DeleteCartAsync(string cartId)
        //{
        //    return await _database.KeyDeleteAsync(cartId);
        //}
    }
}