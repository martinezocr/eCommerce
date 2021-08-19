
namespace eCommerce.Web.Models
{
    /// <summary>
    /// Datos para autenticar a un usuario
    /// </summary>
    public class AuthModel
    {
        /// <summary>
        /// Usuario
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Establece si se debe persistir la cookie
        /// </summary>
        public bool Remember { get; set; }
    }
}
