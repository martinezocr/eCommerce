using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerce.Web
{
    /// <summary>
    /// Datos del usuario
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [JsonProperty]
        public string Username { get; set; }
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [JsonProperty]
        public int UserId { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }
        /// <summary>
        /// Primer nombre del usuario
        /// </summary>
        [JsonProperty]
        public string FirstName { get; set; }
        /// <summary>
        /// Roles del usuario
        /// </summary>
        [JsonProperty]
        public Role[] RoleIds { get; set; }
        /// <summary>
        /// Establece si los datos del usuario persisten o no
        /// </summary>
        [JsonProperty]
        public bool Persist { get; set; }

        /// <summary>
        /// establece si el usuario esta activo o no
        /// </summary>
        [JsonProperty]
        public bool IsLocked { get; set; }
    }

    /// <summary>
    /// Clase para el guardado de la información del usuario
    /// </summary>
    public static class LoginInfo
    {
        public const string CLAIM_FULLNAME = "FullName";
        public const string CLAIM_USER_ID = "UserId";

        /// <summary>
        /// Logea al usuario creando la cookie para utilizarla en el navegador
        /// </summary>
        /// <param name="username">Identificador del usuario</param>
        /// <param name="createPersistentCookie">Establece si se debe persistir la cookie luego de cerrar el navegador</param>
        internal static async Task<UserData> Set(HttpContext httpContext, string username, bool createPersistentCookie = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username");

            var objData = await Data.User.GetAsync(username);
            var objUserInfo = new UserData()
            {
                Username = objData["Username"] as string,
                Name = objData["FullName"] as string,
                FirstName = objData.IsNull("FirstName") ? null : objData["FirstName"] as string,
                UserId = (int)objData["UserId"],
                RoleIds = await Data.User.GetRolesAsync((int)objData["UserId"]),
                Persist = createPersistentCookie
            };

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objUserInfo.Username),
                    new Claim(CLAIM_FULLNAME,objUserInfo.Name),
                    new Claim(CLAIM_USER_ID,objUserInfo.UserId.ToString()),
                    new Claim(ClaimTypes.UserData,JsonConvert.SerializeObject(objUserInfo)),
                };

            if (objUserInfo.RoleIds?.Length > 0)
                foreach (var role in objUserInfo.RoleIds)
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = createPersistentCookie,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            httpContext.Items.Add("UserData", objUserInfo);
            httpContext.Items.Add("UserId", objUserInfo.UserId);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return objUserInfo;

            //_logger.LogInformation("User {Email} logged in at {Time}.",
            //    user.Email, DateTime.UtcNow);
        }

        /// <summary>
        /// Devuelve la información del usuario, obtenida de la cookie
        /// </summary>
        /// <returns>Información del usuario, obtenida de la cookie</returns>
        public static UserData Get(HttpContext httpContext)
        {
            try
            {
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    if (httpContext.Items.ContainsKey("UserData"))
                        return httpContext.Items["UserData"] as UserData;

                    var objData = JsonConvert.DeserializeObject<UserData>(
                        ((ClaimsIdentity)httpContext.User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value
                    );
                    httpContext.Items.Add("UserData", objData);
                    httpContext.Items.Add("UserId", objData.UserId);

                    return objData;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Identificador del usuario logeado
        /// </summary>
        public static int GetUserId(this HttpContext httpContext) => Get(httpContext).UserId;

        /// <summary>
        /// Actualiza los datos almacenados en la cookie del usuario logeado
        /// </summary>
        //public static void Update()
        //{
        //    var objCookieData = Get();
        //    var objData = Data.User.Get(objCookieData.UserId);
        //    var objCookieInfo = new UserData()
        //    {
        //        Username = objData["Username"] as string,
        //        Name = (objData["FirstName"] as string) + ' ' + (objData["LastName"] as string),
        //        UserId = objCookieData.UserId,
        //        RoleIds = Data.User.GetRoles(objCookieData.UserId),
        //        Persist = objCookieData.Persist
        //    };

        //    HttpCookie objCookie = FormsAuthentication.GetAuthCookie(objCookieInfo.Username, objCookieData.Persist);
        //    FormsAuthenticationTicket objTicket = FormsAuthentication.Decrypt(objCookie.Value);
        //    FormsAuthenticationTicket objNewTicket = new FormsAuthenticationTicket(
        //        objTicket.Version, objTicket.Name, objTicket.IssueDate, objTicket.Expiration,
        //        false, JsonConvert.SerializeObject(objCookieInfo), objTicket.CookiePath
        //    );

        //    objCookie.Value = FormsAuthentication.Encrypt(objNewTicket);
        //    HttpContext.Current.Response.Cookies.Add(objCookie);
        //}
    }
}