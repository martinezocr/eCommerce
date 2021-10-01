using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eCommerce.Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using eCommerce.Web.Data;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la administración de los datos de carrito de compras
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _hostEnv;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public ProductController(ILogger<ProductController> logger, IWebHostEnvironment hostEnv)
        {
            _logger = logger;
            _hostEnv = hostEnv;
        }

        /// <summary>
        /// Listado de todos los usuarios de la aplicación
        /// </summary>
        /// <param name="queryData">Filtros de la consulta</param>
        /// <returns>Listado de los usuarios</returns>
        [HttpPost("list")]
        [Authorize(Roles = "Admin")]
        public async Task<DataTableModel> ListAsync([FromBody] QueryDataModel<Data.Product.Filter, Data.Product.Fields> queryData)
        {
            (var objList, int recordCount) = await Product.ListAsync(HttpContext.GetUserId(), queryData.Order, queryData.OrderAsc, queryData.Filter, queryData.From, queryData.Length);

            return new DataTableModel()
            {
                RecordsCount = recordCount,
                Data = objList
            };
        }

        [HttpPut()]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(104_857_600)] //100MB
        public async Task<Product.SaveResult> PutAsync()
        {
            var files = Request.Form.Files;
            var data = JsonConvert.DeserializeObject<ProductModel>(Request.Form["data"]);

            if (data.Images?.Length > 0)
                foreach (var item in data.Images.Where(p => !p.ProductId.HasValue))
                    item.File = files.Where(p => p.FileName == item.Filename).First() as FormFile;
            string folderUpload = Path.Combine(_hostEnv.WebRootPath, Const.FOLDER_PRODUCT_IMAGEFILE);
            return await Product.SaveAsync(HttpContext.GetUserId(), data, folderUpload);
        }

        /// <summary>
        /// devuelve la información de un producto
        /// </summary>
        /// <param name="productId">identificador del producto</param>
        /// <returns></returns>
        [HttpGet("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ProductModel> GetAsync(int productId)
        {
            return await Product.GetAsync(HttpContext.GetUserId(), productId);
        }

        /// <summary>
        /// elimina un producto
        /// </summary>
        /// <param name="productId">identificador del producto</param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        public async Task<bool> DeleteAsync(int productId)
        {
            var path = Path.Combine(_hostEnv.WebRootPath, Const.FOLDER_PRODUCT_IMAGEFILE);
            return await Product.DeleteAsync(HttpContext.GetUserId(), productId, path);
        }
    }
}