using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eCommerce.Web.Models;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace eCommerce.Web.Controllers
{
    /// <summary>
    /// Controlador para la administración de los datos de los usuarios
    /// </summary>
    [ApiController]
    [Route(Const.ROOT_CONTROLLER)]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="logger"></param>
        public UserController(ILogger<UserController> logger) => _logger = logger;

        /// <summary>
        /// Entidad para cambiar de contraseña
        /// </summary>
        public class UserDataPassword
        {
            /// <summary>
            /// Contraseña actual
            /// </summary>
            public string Current { get; set; }
            /// <summary>
            /// Nueva contraseña
            /// </summary>
            public string New { get; set; }
        }

        /// <summary>
        /// Listado de todos los usuarios de la aplicación
        /// </summary>
        /// <param name="queryData">Filtros de la consulta</param>
        /// <returns>Listado de los usuarios</returns>
        [HttpPost("list")]
        [Authorize(Roles = "Admin")]
        public DataTableModel List([FromBody] QueryDataModel<Data.User.Filter, Data.User.Fields> queryData)
        {
            var objList = Data.User.List(queryData.Order, queryData.OrderAsc, queryData.Filter, queryData.From, queryData.Length, out int RecordCount);

            return new DataTableModel()
            {
                RecordsCount = RecordCount,
                Data = objList
            };
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="userId"> Identificador del usuario</param>
        [HttpDelete("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int userId) => Data.User.Delete(HttpContext.GetUserId(), userId);

        /// <summary>
        /// Devuelve los datos del usuario actual
        /// </summary>
        /// <returns>Datos del usuario actual</returns>
        [HttpGet()]
        public UserData Get() => LoginInfo.Get(HttpContext);

        /// <summary>
        /// Devuelve los datos de un usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>Datos del usuario</returns>
        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public UserModel Get(int userId)
        {
            var objUserDR = Data.User.Get(userId);
            return new UserModel
            {
                UserId = userId,
                Username = objUserDR["Username"] as string,
                IsActive = (bool)objUserDR["IsActive"],
                Roles = Data.User.GetRoles(userId),
                Email = objUserDR["Email"] as string,
                EnterpriseId = objUserDR.IsNull("EnterpriseId") ? (int?)null : (int)objUserDR["EnterpriseId"],
            };
        }

        /// <summary>
        /// Autenticación de un usuario
        /// </summary>
        /// <param name="data">Información del usuario</param>
        /// <returns>Resultado de la autenticación</returns>
        [HttpPost("auth")]
        public async Task<UserData> Auth([FromBody] AuthModel data)
        {
            Data.User.ValidateResult result = Data.User.Validate(data.Username, data.Password);

            return result switch
            {
                Data.User.ValidateResult.NotExists => null,
                Data.User.ValidateResult.IsLocked => new UserData() { IsLocked = true },
                _ => await LoginInfo.Set(HttpContext, data.Username, data.Remember),
            };
        }

        /// <summary>
        /// Deslogea al usuario
        /// </summary>
        [HttpGet("logout")]
        public void Logout() => HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// Graba los datos de un usuario
        /// </summary>
        /// <param name="data"></param>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public bool Put([FromBody] UserModel data) => Data.User.Save(HttpContext.GetUserId(), data);

        /// <summary>
        /// Cambio de contraseña
        /// </summary>
        /// <param name="data">Datos de la contraseña actual y la nueva</param>
        /// <returns>Resultado del cambio de la contraseña</returns>
        [HttpPost("password")]
        [Authorize]
        public bool UpdatePassword([FromBody] UserDataPassword data) =>
            Data.User.UpdatePassword(HttpContext.GetUserId(), data.Current, data.New);

        /// <summary>
        /// Listado de los Usuarios
        /// </summary>
        /// <returns>Listado de los usuarios</returns>
        [HttpGet("all")]
        [Authorize]
        public DataTable List() => Data.User.ListAll(HttpContext.GetUserId());

        /// <summary>
        /// genera la descargar del excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-excel")]
        [Authorize(Roles = "Admin")]
        public IActionResult DownloadExcel()
        {
            return File(Helpers.Excel.ToExcel(Data.User.ListForExcel(HttpContext.GetUserId()), "Usuarios"), Helpers.Excel.MIME_XLSX, "Usuarios.xlsx");
        }
    }
}