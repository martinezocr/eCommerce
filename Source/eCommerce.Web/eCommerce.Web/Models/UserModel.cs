namespace eCommerce.Web.Models
{
    /// <summary>
    /// Entidad para la representación de un usuario
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// identificador de la empresa asociada al usuario
        /// </summary>
        public int? EnterpriseId { get; set; }

        /// <summary>
        /// Estado del usuario
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Roles del usuario
        /// </summary>
        public Role[] Roles { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        public string Password { get; set; }
    }
}