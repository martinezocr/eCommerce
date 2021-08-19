namespace eCommerce.Web
{
    /// <summary>
    /// Roles de la aplicación
    /// </summary>
    public enum Role : byte
    {
        /// <summary>
        /// Administrador
        /// </summary>
        Admin = 255,
        /// <summary>
        /// usuario comun
        /// </summary>
        User = 1
    }

    /// <summary>
    /// Tipos de solicitud
    /// </summary>
    public enum RequestType : byte
    {
        Normal = 1,
        Urgent = 2
    }

    /// <summary>
    /// tipos de moneda
    /// </summary>
    public enum Currency : byte
    {
        Pesos = 1,
        Euro = 2,
        UF = 3,
        USD = 4
    }
}
