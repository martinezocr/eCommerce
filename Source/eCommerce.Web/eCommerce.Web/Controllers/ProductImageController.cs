using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la devolución de los archivos
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    public class ProductImageController : ControllerBase
    {
        private readonly ILogger<ProductImageController> _logger;
        private readonly IWebHostEnvironment _hostEnv;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public ProductImageController(ILogger<ProductImageController> logger, IWebHostEnvironment hostEnv)
        {
            _logger = logger;
            _hostEnv = hostEnv;
        }

        /// <summary>
        /// Devuelve el archivo solicitado
        /// </summary>
        /// <param name="fileId">Identificador del archivo</param>
        /// <returns>Archivo</returns>
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> Get(Guid fileId)
        {
            var file = await Data.ProductImage.Get(fileId);
            if (file == null)
                return NotFound();

            var path = Path.Combine(_hostEnv.WebRootPath, Const.FOLDER_PRODUCT_IMAGEFILE, fileId.ToString());

            if (!System.IO.File.Exists(path))
                return NotFound();

            return new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read), file.MimeType)
            {
                FileDownloadName = file.Filename,
                EnableRangeProcessing = !file.IsImage()
            };
        }
    }
}