using eCommerce.Web.Models;
using CustomSqlClient.Net.Core;
using System;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace eCommerce.Web.Data
{
    /// <summary>
    /// Clase para el manejo de la información de un usuario
    /// </summary>
    public static class User
    {
        #region Consulta

        /// <summary>
        /// Campos de la consulta
        /// </summary>
        public enum Fields
        {
            UserId = 0,
            Username = 1,
            FirstName = 2,
            LastName = 3,
            IsActive = 4,
            LoggedOn = 5
        }

        /// <summary>
        /// Clase para definir los valores de los filtros
        /// </summary>
        [Serializable]
        public class Filter
        {
            /// <summary>
            /// Filtro para el campo "Username"
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Filtro para el campo "FirstName"
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// Filtro para el campo "LastName"
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// Filtro para el campo "IsActive"
            /// </summary>
            public bool? IsActive { get; set; }

            /// <summary>
            /// Búsqueda por texto libre
            /// </summary>
            public string FreeText { get; set; }
        }

        private const string CTE =
            ";WITH CTE AS" +
            " (" +
                " SELECT T.[UserId], ROW_NUMBER() OVER(ORDER BY {0} {1}) AS RowNumber" +   //{0} Campo - {1} Orden: ASC/DESC
                " FROM [User] AS T" +
                " WHERE T.IsDeleted = 0 {2}" +   //{2} Filtro (WHERE)
            " )";

        private const string SELECT_ALL =
            "{0} SELECT " +                                              //{0} - CTE
                " CTE.[UserId], T.[Username], T.[FirstName], T.[LastName]," +
                " T.[IsActive], T.[LoggedOn]" +
            " FROM CTE" +
            " INNER JOIN [User] AS T ON T.[UserId] = CTE.[UserId]";

        private const string SELECT = SELECT_ALL +
            " WHERE CTE.RowNumber BETWEEN {1} AND {2}";	                //{1} Desde - {2} Hasta

        private const string ORDER = " ORDER BY CTE.RowNumber";
        private const string SELECTCOUNT = "SELECT COUNT(1) AS rows FROM [User] AS T WHERE IsDeleted = 0";

        /// <summary>
        /// Obtiene la lista de registros paginada, ordenada y filtrada
        /// </summary>
        /// <param name="userId">Usuario que genera la consulta</param>
        /// <param name="orderField">Campo de orden</param>
        /// <param name="orderAscendant">Orden ascendente</param>
        /// <param name="from">Registro desde el cual traer los datos (en base cero)</param>
        /// <param name="length">Cantidad de registros a obtener</param>
        /// <param name="filter">Filtros a utilizar</param>
        /// <param name="recordCount">Cantidad de registros encontrados con los filtros establecidos</param>
        /// <returns>Registros obtenidos con los filtros y orden seleccionados</returns>
        public static async Task<(DataTable, int)> ListAsync(int userId, Fields? orderField, bool? orderAscendant, Filter filter, int? from, int? length)
        {
            if (!await HasRoleAsync(userId, Role.Admin))
                throw new SecurityException();

            if (from.HasValue != length.HasValue)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (length.HasValue && length > 100)
                throw new ArgumentOutOfRangeException(nameof(length));

            string strOrderField;
            if (orderField.HasValue)
                strOrderField = orderField.ToString();
            else
                strOrderField = Fields.Username.ToString();

            using SqlCommand objSqlCmd = new SqlCommand();
            //Filtro
            string strFilter = string.Empty;
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Username))
                {
                    strFilter += " AND [Username] LIKE '%' + @Username + '%'";
                    objSqlCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = filter.Username;
                }
                if (!string.IsNullOrWhiteSpace(filter.FirstName))
                {
                    strFilter += " AND [FirstName] LIKE '%' + @FirstName + '%'";
                    objSqlCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 150).Value = filter.FirstName;
                }
                if (!string.IsNullOrWhiteSpace(filter.LastName))
                {
                    strFilter += " AND [LastName] LIKE '%' + @LastName + '%'";
                    objSqlCmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 150).Value = filter.LastName;
                }
                if (filter.IsActive.HasValue)
                {
                    strFilter += " AND T.[IsActive] = @IsActive";
                    objSqlCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = filter.IsActive.Value;
                }
                if (!string.IsNullOrWhiteSpace(filter.FreeText))
                {
                    strFilter += " AND CONTAINS(T.*, @FreeText)";
                    objSqlCmd.Parameters.Add("@FreeText", SqlDbType.NVarChar, 1000).Value = SqlCommand.MakeContainsSearchCondition(filter.FreeText, ' ', SqlCommand.FTSOperators.AND, true);
                }
            }
            string strCteWithParams = string.Format(CTE, strOrderField, !orderAscendant.HasValue || orderAscendant.Value ? "ASC" : "DESC", strFilter);
            objSqlCmd.CommandType = CommandType.Text;

            int recordCount = 0;
            if (from.HasValue)
            {
                string strSelectWithParams = string.Format(SELECT, strCteWithParams, from + 1, from + length) + ORDER;

                objSqlCmd.CommandText = SELECTCOUNT + strFilter + strSelectWithParams;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(objSqlCmd.CommandText);
#endif
                DataSet objDS = await objSqlCmd.ExecuteDataSetAsync();
                recordCount = (int)objDS.Tables[0].Rows[0][0];

                return (objDS.Tables[1], recordCount);
            }
            else
            {
                string strSelectWithParams = string.Format(SELECT_ALL, strCteWithParams) + ORDER;
                objSqlCmd.CommandText = strSelectWithParams;
                recordCount = -1;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(strSelectWithParams);
#endif
                return (await objSqlCmd.ExecuteDataTableAsync(), recordCount);
            }
        }

        #endregion

        #region Seguridad

        /// <summary>
        /// Devuelve los datos de un usuario
        /// </summary>
        /// <param name="editorUserId">Usuario administrador que realiza la acción</param>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>Datos del usuario</returns>
        public static DataRow Get(int editorUserId, int userId)
        {
            using var objCmd = new SqlCommand("User_Get", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@EditorUserId", SqlDbType.Int).Value = editorUserId;
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            return objCmd.ExecuteDataRow();
        }

        /// <summary>
        /// Devuelve los datos básicos de un usuario
        /// </summary>
        /// <param name="username">Identificador del usuario</param>
        /// <returns>Datos básicos del usuario</returns>
        public static DataRow Get(string username)
        {
            using var objCmd = new SqlCommand("User_GetByUsername", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            return objCmd.ExecuteDataRow();
        }

        /// <summary>
        /// Devuelve los roles de un usuario
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <returns>Rol del suario</returns>
        public static async Task<string[]> GetRolesAsync(string username)
        {
            using var objCmd = new SqlCommand("User_GetRoles", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            return await objCmd.ExecuteArrayStringAsync();
        }

        /// <summary>
        /// Devuelve los roles de un usuario
        /// </summary>
        /// <param name="userId">Identificador del suario</param>
        /// <returns>Roles del suario</returns>
        public static async Task<Role[]> GetRolesAsync(int userId)
        {
            using var objCmd = new SqlCommand("User_GetRolesByUserId", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            return (await objCmd.ExecuteArrayByteAsync()).Select(r => (Web.Role)r).ToArray();
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="editorUserId">Usuario administrador que realiza la acción</param>
        /// <param name="userId">Identificador del usuario</param>
        public static async Task<bool> DeleteAsync(int editorUserId, int userId)
        {
            using var objCmd = new SqlCommand("User_Delete", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@EditorUserId", SqlDbType.Int).Value = editorUserId;
            int result = await objCmd.ExecuteReturnInt32Async();

            if (result == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return result > 0;
        }

        /// <summary>
        /// Desactiva un usuario
        /// </summary>
        /// <param name="editorUserId">Usuario administrador que realiza la acción</param>
        /// <param name="username">Identificador del usuario</param>
        /// <returns>Si se pudo realizar la acción con éxito</returns>
        public static async Task<bool> DeactivateAsync(int editorUserId, string username)
        {
            using var objCmd = new SqlCommand("User_Deactivate", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@EditorUserId", SqlDbType.Int).Value = editorUserId;
            int result = await objCmd.ExecuteReturnInt32Async();

            if (result == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return result > 0;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="oldPassword">Contraseña anterior</param>
        /// <param name="newPassword">Contraseña nueva</param>
        /// <returns>Si se pudo realizar la acción con éxito</returns>
        public static async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            using var objCmd = new SqlCommand("User_ChangePassword", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@oldPassword", SqlDbType.NVarChar, 255).Value = oldPassword;
            objCmd.Parameters.Add("@newPassword", SqlDbType.NVarChar, 255).Value = newPassword;
            return await objCmd.ExecuteReturnInt32Async() > 0;
        }

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>Resultado de la autenticación</returns>
        public static async Task<bool> ValidateAsync(string username, string password)
        {
            using var objCmd = new SqlCommand("User_Validate", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@password", SqlDbType.NVarChar, 255).Value = password;
            return await objCmd.ExecuteReturnInt32Async() > 0;
        }

        #endregion

        /// <summary>
        /// Graba los datos de un usuario
        /// </summary>
        /// <param name="data">Datos del usuario</param>
        /// <param name="userId">Identificador del usuario</param>
        internal static async Task<bool> SaveAsync(int userId, UserModel data)
        {
            using var objCmd = new SqlCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            if (data.UserId.HasValue && data.UserId.Value != 0)
            {
                objCmd.CommandText = "User_Update";
                objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = data.UserId.Value;
            }
            else
                objCmd.CommandText = "User_Add";
            objCmd.Parameters.Add("@EditorUserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = data.Username;
            objCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 150).Value = data.FirstName;
            objCmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = data.LastName;
            objCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = data.IsActive;
            if (!string.IsNullOrEmpty(data.Password))
                objCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = data.Password;
            if (data.Roles != null && data.Roles.Length > 0)
                objCmd.Parameters.Add("@Roles", SqlDbType.VarChar, 100).Value = string.Join(",", data.Roles.Select(r => ((byte)r).ToString()));
            int result = await objCmd.ExecuteReturnInt32Async();

            if (result == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return result == 0;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>Devuelve si se pudo cambiar la contraseña</returns>
        internal static async Task<bool> UpdatePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            using var objCmd = new SqlCommand("User_UpdatePassword", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@CurrentPassword", SqlDbType.NVarChar, 100).Value = currentPassword;
            objCmd.Parameters.Add("@newPassword", SqlDbType.NVarChar, 100).Value = newPassword;
            return await objCmd.ExecuteReturnInt32Async() > 0;
        }

        /// <summary>
        /// Devuelve el listado de usuarios
        /// </summary>
        /// <returns>Listado de usuario</returns>
        public static async Task<DataTable> ListAllAsync(int userId)
        {
            using var objCmd = new SqlCommand("User_List", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            var result = objCmd.Parameters.Add("@Result", SqlDbType.Int);
            result.Direction = ParameterDirection.ReturnValue;

            using var objDT = await objCmd.ExecuteDataTableAsync();

            if (result.Value != null && (int)result.Value == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return objDT;
        }

        /// <summary>
        /// Devuelve si un usuario tiene un rol específico
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="role">Role que debe tener</param>
        /// <returns><c>true</c> si el usuario tiene asignado ese rol o <c>false</c> en caso contrario</returns>
        public static async Task<bool> HasRoleAsync(int userId, Role role)
        {
            using var objCmd = new SqlCommand("User_HasRole", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@RoleId", SqlDbType.TinyInt).Value = (byte)role;
            return await objCmd.ExecuteReturnInt32Async() != 0;
        }

        /// <summary>
        /// genera un excel de los usuarios
        /// </summary>
        /// <param name="userId">identificador del usuario que realiza la acción</param>
        /// <returns></returns>
        public static async Task<DataTable> ListForExcelAsync(int userId)
        {
            if (!await HasRoleAsync(userId, Role.Admin))
                throw new SecurityException();
            using var cmd = new SqlCommand("User_ListForExcel", CommandType.StoredProcedure);
            return await cmd.ExecuteDataTableAsync();
        }
    }
}