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
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Listado de todos las categorías de la aplicación
        /// </summary>
        /// <param name="queryData">Filtros de la consulta</param>
        [HttpPost("list")]
        [Authorize(Roles = "Admin")]
        public async Task<DataTableModel> ListAsync([FromBody] QueryDataModel<Category.Filter, Category.Fields> queryData)
        {
            (var objList, int recordCount) = await Category.ListAsync(HttpContext.GetUserId(), queryData.Order, queryData.OrderAsc, queryData.Filter, queryData.From, queryData.Length);

            return new DataTableModel()
            {
                RecordsCount = recordCount,
                Data = objList
            };
        }

        /// <summary>
        /// graba o actualiza una categoría en la base de datos
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut()]
        [Authorize(Roles = "Admin")]
        public async Task<bool> PutAsync([FromBody] CategoryModel data)
        {
            return await Category.SaveAsync(HttpContext.GetUserId(), data);
        }

        /// <summary>
        /// devuelve la información de una categoría
        /// </summary>
        /// <param name="categoryId">identificador de la categoría</param>
        /// <returns></returns>
        [HttpGet("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<CategoryModel> GetAsync(int categoryId)
        {
            return await Category.GetAsync(HttpContext.GetUserId(), categoryId);
        }

        /// <summary>
        /// elimina un producto
        /// </summary>
        /// <param name="categoryId">identificador de la categoría</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId}")]
        public async Task<bool> DeleteCartAsync(int categoryId)
        {
            return await Category.DeleteAsync(HttpContext.GetUserId(), categoryId);
        }

        /// <summary>
        /// devuelve la lista de categorías
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-all")]
        public async Task<CategoryModel[]> ListAllAsync()
        {
            return await Category.ListAllAsync();
        }
    }
}