using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eCommerce.Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using eCommerce.Web.Data;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la administración de los datos de carrito de compras
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    public class BrandController : ControllerBase
    {
        private readonly ILogger<BrandController> _logger;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public BrandController(ILogger<BrandController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Listado de todos los usuarios de la aplicación
        /// </summary>
        /// <param name="queryData">Filtros de la consulta</param>
        /// <returns>Listado de los usuarios</returns>
        [HttpPost("list")]
        [Authorize(Roles = "Admin")]
        public async Task<DataTableModel> ListAsync([FromBody] QueryDataModel<Brand.Filter, Brand.Fields> queryData)
        {
            (var objList, int recordCount) = await Brand.ListAsync(HttpContext.GetUserId(), queryData.Order, queryData.OrderAsc, queryData.Filter, queryData.From, queryData.Length);

            return new DataTableModel()
            {
                RecordsCount = recordCount,
                Data = objList
            };
        }

        [HttpPut()]
        [Authorize(Roles = "Admin")]
        public async Task<bool> PutAsync([FromBody] BrandModel data)
        {
            return await Brand.SaveAsync(HttpContext.GetUserId(), data);
        }

        /// <summary>
        /// devuelve la información de una marca
        /// </summary>
        /// <param name="brandId">identificador de la marca</param>
        /// <returns></returns>
        [HttpGet("{brandId}")]
        [Authorize(Roles = "Admin")]
        public async Task<BrandModel> GetAsync(int brandId)
        {
            return await Brand.GetAsync(HttpContext.GetUserId(), brandId);
        }

        /// <summary>
        /// elimina una marca
        /// </summary>
        /// <param name="brandId">identificador de la marca</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{brandId}")]
        public async Task<bool> DeleteCartAsync(int brandId)
        {
            return await Brand.DeleteAsync(HttpContext.GetUserId(), brandId);
        }

        /// <summary>
        /// devuelve la lista de marcas
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-all")]
        public async Task<BrandModel[]> ListAllAsync()
        {
            return await Brand.ListAllAsync();
        }
    }
}