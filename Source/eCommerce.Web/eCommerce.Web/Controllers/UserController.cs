using eCommerce.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

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
        public async Task<DataTableModel> ListAsync([FromBody] QueryDataModel<Data.User.Filter, Data.User.Fields> queryData)
        {
            (var objList, int recordCount) = await Data.User.ListAsync(HttpContext.GetUserId(), queryData.Order, queryData.OrderAsc, queryData.Filter, queryData.From, queryData.Length);

            return new DataTableModel()
            {
                RecordsCount = recordCount,
                Data = objList
            };
        }

        /// <summary>
        /// Desactiva un usuario
        /// </summary>
        /// <param name="userId"> Identificador del usuario</param>
        [HttpDelete("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task DeleteAsync(int userId) => await Data.User.DeleteAsync(HttpContext.GetUserId(), userId);

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
        public async Task<UserModel> GetAsync(int userId)
        {
            var objUserDR = Data.User.Get(HttpContext.GetUserId(), userId);
            return new UserModel
            {
                UserId = userId,
                Username = objUserDR["Username"] as string,
                FirstName = objUserDR["Firstname"] as string,
                LastName = objUserDR["Lastname"] as string,
                IsActive = (bool)objUserDR["IsActive"],
                Roles = await Data.User.GetRolesAsync(userId)
            };
        }

        /// <summary>
        /// Autenticación de un usuario
        /// </summary>
        /// <param name="data">Información del usuario</param>
        /// <returns>Resultado de la autenticación</returns>
        [HttpPost("auth")]
        public async Task<UserData> AuthAsync([FromBody] AuthModel data)
        {
            bool bolOk = await Data.User.ValidateAsync(data.Username, data.Password);
            if (!bolOk)
                return null;

            return await LoginInfo.Set(HttpContext, data.Username, data.Remember);
        }

        /// <summary>
        /// Deslogea al usuario
        /// </summary>
        [HttpGet("logout")]
        public async Task LogoutAsync() => await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// Graba los datos de un usuario
        /// </summary>
        /// <param name="data"></param>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<bool> PutAsync([FromBody] UserModel data) => await Data.User.SaveAsync(HttpContext.GetUserId(), data);

        /// <summary>
        /// Cambio de contraseña
        /// </summary>
        /// <param name="data">Datos de la contraseña actual y la nueva</param>
        /// <returns>Resultado del cambio de la contraseña</returns>
        [HttpPost("password")]
        public async Task<bool> UpdatePasswordAsync([FromBody] UserDataPassword data) =>
            await Data.User.UpdatePasswordAsync(HttpContext.GetUserId(), data.Current, data.New);

        /// <summary>
        /// Listado de los Usuarios
        /// </summary>
        /// <returns>Listado de los usuarios</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<DataTable> ListAllAsync() => await Data.User.ListAllAsync(HttpContext.GetUserId());

        /// <summary>
        /// devuelve un excel con datos de la tabla de usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadExcelAsync()
        {
            return File(Helpers.Excel.ToExcel(await Data.User.ListForExcelAsync(HttpContext.GetUserId()), "Usuarios"), Helpers.Excel.MIME_XLSX, "Usuarios.xlsx");
        }
    }
}