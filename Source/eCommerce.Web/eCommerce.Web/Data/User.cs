using CustomSqlClient.Net.Core;
using eCommerce.Web.Models;
using System;
using System.Data;
using System.Linq;
using System.Security;

namespace eCommerce.Web.Data
{
    /// <summary>
    /// Clase para el manejo de la información de un usuario
    /// </summary>
    public static class User
    {
        /// <summary>
        /// enumerador para los resultados de la validación para la autenticación
        /// </summary>
        public enum ValidateResult : int
        {
            /// <summary>
            /// el usuario no existe
            /// </summary>
            NotExists = 0,
            /// <summary>
            /// el usuario está desactivado
            /// </summary>
            IsLocked = -1,
            /// <summary>
            /// todo bien
            /// </summary>
            Ok = 1
        }

        #region Consulta

        /// <summary>
        /// Campos de la consulta
        /// </summary>
        public enum Fields
        {
            UserId = 0,
            Username = 1,
            IsActive = 2,
            LoggedOn = 3
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
                " FROM [User] AS T INNER JOIN Enterprise E ON T.EnterpriseId = E.EnterpriseId" +
                " WHERE T.UserId > 0 {2}" +   //{2} Filtro (WHERE)
                                              //" AND not exists(select 1 from [User_Role] UR where UR.UserId = T.UserId and UR.RoleId = 3 ) {2}" +   //{2} Filtro (WHERE)
            " )";

        private const string SELECT_ALL =
            "{0} SELECT " +                                              //{0} - CTE
                " CTE.[UserId], T.[Username],E.Name as Enterprise, " +
                " T.[IsActive], T.[LoggedOn]" +
            " FROM CTE" +
            " INNER JOIN [User] AS T ON T.[UserId] = CTE.[UserId]" +
            " INNER JOIN Enterprise E ON T.EnterpriseId = E.EnterpriseId ";

        private const string SELECT = SELECT_ALL +
            " WHERE CTE.RowNumber BETWEEN {1} AND {2}";	                //{1} Desde - {2} Hasta

        private const string ORDER = " ORDER BY CTE.RowNumber";
        private const string SELECTCOUNT = "SELECT COUNT(1) AS rows FROM [User] AS T INNER JOIN Enterprise AS E ON T.EnterpriseId = E.EnterpriseId WHERE T.UserId > 0";

        /// <summary>
        /// Obtiene la lista de registros paginada, ordenada y filtrada
        /// </summary>
        /// <param name="orderField">Campo de orden</param>
        /// <param name="orderAscendant">Orden ascendente</param>
        /// <param name="from">Registro desde el cual traer los datos (en base cero)</param>
        /// <param name="length">Cantidad de registros a obtener</param>
        /// <param name="filter">Filtros a utilizar</param>
        /// <param name="recordCount">Cantidad de registros encontrados con los filtros establecidos</param>
        /// <returns>Registros obtenidos con los filtros y orden seleccionados</returns>
        public static DataTable List(Fields? orderField, bool? orderAscendant, Filter filter, int? from, int? length, out int recordCount)
        {
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

            if (from.HasValue)
            {
                string strSelectWithParams = string.Format(SELECT, strCteWithParams, from + 1, from + length) + ORDER;

                objSqlCmd.CommandText = SELECTCOUNT + strFilter + strSelectWithParams;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(objSqlCmd.CommandText);
#endif
                DataSet objDS = objSqlCmd.ExecuteDataSet();
                recordCount = (int)objDS.Tables[0].Rows[0][0];

                return objDS.Tables[1];
            }
            else
            {
                string strSelectWithParams = string.Format(SELECT_ALL, strCteWithParams) + ORDER;
                objSqlCmd.CommandText = strSelectWithParams;
                recordCount = -1;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(strSelectWithParams);
#endif
                return objSqlCmd.ExecuteDataTable();
            }
        }

        #endregion

        #region Seguridad

        /// <summary>
        /// Devuelve los datos de un usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>Datos del usuario</returns>
        public static DataRow Get(int userId)
        {
            using var objCmd = new SqlCommand("User_Get", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
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
        public static string[] GetRoles(string username)
        {
            using var objCmd = new SqlCommand("User_GetRoles", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            return objCmd.ExecuteArrayString();
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="editorUserId">Usuario administrador que realiza la acción</param>
        /// <param name="userId">Identificador del usuario</param>
        public static bool Delete(int editorUserId, int userId)
        {
            using var objCmd = new SqlCommand("User_Delete", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@EditorUserId", SqlDbType.Int).Value = editorUserId;
            int result = objCmd.ExecuteReturnInt32();

            if (result == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return result > 0;
        }

        /// <summary>
        /// Devuelve los roles de un usuario
        /// </summary>
        /// <param name="userId">Identificador del suario</param>
        /// <returns>Roles del suario</returns>
        public static Role[] GetRoles(int userId)
        {
            using var objCmd = new SqlCommand("User_GetRolesByUserId", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            return objCmd.ExecuteArrayByte().Select(r => (Role)r).ToArray();
        }

        /// <summary>
        /// Devuelve si un usuario está en un rol
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="roleName">Rol</param>
        /// <returns>Devuelve si un usuario está en un rol</returns>
        public static bool IsInRole(string username, string roleName)
        {
            using var objCmd = new SqlCommand("User_IsInRole", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@roleName", SqlDbType.NVarChar, 20).Value = roleName;
            return objCmd.ExecuteReturnInt32() > 0;
        }

        /// <summary>
        /// Devuelve si un usuario tiene un rol específico
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="role">Role que debe tener</param>
        /// <returns><c>true</c> si el usuario tiene asignado ese rol o <c>false</c> en caso contrario</returns>
        public static bool HasRole(int userId, Role role)
        {
            using var objCmd = new SqlCommand("User_HasRole", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@RoleId", SqlDbType.TinyInt).Value = (byte)role;
            return objCmd.ExecuteReturnInt32() != 0;
        }

        /// <summary>
        /// Desactiva un usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        public static void Deactivate(int userId)
        {
            using var objCmd = new SqlCommand("User_DeactivateByUserId", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Desactiva un usuario
        /// </summary>
        /// <param name="username">Identificador del usuario</param>
        /// <returns>Si se pudo realizar la acción con éxito</returns>
        public static bool Deactivate(string username)
        {
            using var objCmd = new SqlCommand("User_Deactivate", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            return objCmd.ExecuteReturnInt32() > 0;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="oldPassword">Contraseña anterior</param>
        /// <param name="newPassword">Contraseña nueva</param>
        /// <returns>Si se pudo realizar la acción con éxito</returns>
        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using var objCmd = new SqlCommand("User_ChangePassword", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@oldPassword", SqlDbType.NVarChar, 255).Value = oldPassword;
            objCmd.Parameters.Add("@newPassword", SqlDbType.NVarChar, 255).Value = newPassword;
            return objCmd.ExecuteReturnInt32() > 0;
        }

        /// <summary>
        /// Desbloquea un usuario
        /// </summary>
        /// <param name="username">Identificador del usuario</param>
        /// <returns>Si se pudo realizar la acción con éxito</returns>
        public static bool Unlock(string username)
        {
            using var objCmd = new SqlCommand("User_Unlock", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            return objCmd.ExecuteReturnInt32() > 0;
        }

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>Resultado de la autenticación</returns>
        public static ValidateResult Validate(string username, string password)
        {
            using var objCmd = new SqlCommand("User_Validate", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@username", SqlDbType.NVarChar, 255).Value = username;
            objCmd.Parameters.Add("@password", SqlDbType.NVarChar, 255).Value = password;
            return (ValidateResult)objCmd.ExecuteReturnInt32();
        }

        #endregion

        /// <summary>
        /// Graba los datos de un usuario
        /// </summary>
        /// <param name="data">Datos del usuario</param>
        /// <param name="userId">Identificador del usuario</param>
        internal static bool Save(int userId, UserModel data)
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
            objCmd.Parameters.Add("@Email", SqlDbType.NVarChar, -1).Value = data.Email;
            objCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = data.IsActive;
            objCmd.Parameters.Add("@EnterpriseId", SqlDbType.Int).Value = data.EnterpriseId;
            if (!string.IsNullOrEmpty(data.Password))
                objCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = data.Password;
            objCmd.Parameters.Add("@Roles", SqlDbType.VarChar, 100).Value = String.Join(",", data.Roles.Select(r => ((byte)r).ToString()));
            return objCmd.ExecuteReturnInt32() == 0;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>Devuelve si se pudo cambiar la contraseña</returns>
        internal static bool UpdatePassword(int userId, string currentPassword, string newPassword)
        {
            using var objCmd = new SqlCommand("User_UpdatePassword", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@CurrentPassword", SqlDbType.NVarChar, 100).Value = currentPassword;
            objCmd.Parameters.Add("@newPassword", SqlDbType.NVarChar, 100).Value = newPassword;
            return objCmd.ExecuteReturnInt32() > 0;
        }

        /// <summary>
        /// Devuelve el listado de usuarios
        /// </summary>
        /// <returns>Listado de usuario</returns>
        public static DataTable ListAll(int userId)
        {
            if (!HasRole(userId, Role.Admin))
                throw new SecurityException();
            using var cmd = new SqlCommand("User_List", CommandType.StoredProcedure);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// devuelve la lista de usuario para generar el excel de descarga
        /// </summary>
        /// <param name="userId">usuario que realiza la acción</param>
        /// <returns></returns>
        public static DataTable ListForExcel(int userId)
        {
            if (!HasRole(userId, Role.Admin))
                throw new SecurityException();
            using var cmd = new SqlCommand("User_ListForExcel", CommandType.StoredProcedure);
            return cmd.ExecuteDataTable();
        }
    }
}