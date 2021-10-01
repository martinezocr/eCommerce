﻿using CustomSqlClient.Net.Core;
using eCommerce.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace eCommerce.Web.Data
{
    /// <summary>
    /// clase para el manejo de datos de las marcas
    /// </summary>
    public static class Category
    {
        #region Administracion
        /// <summary>
        /// Campos de la consulta
        /// </summary>
        public enum Fields
        {
            CategoryId = 0,
            Name = 1,
        }

        /// <summary>
        /// Clase para definir los valores de los filtros
        /// </summary>
        [Serializable]
        public class Filter
        {
            /// <summary>
            /// Búsqueda por texto libre
            /// </summary>
            public string FreeText { get; set; }
        }

        private const string SELECT_ALL =
            @" ;SELECT C.CategoryId, C.[Name]
                FROM Categrory C
                {2} 
                ORDER BY {0} {1} ";

        private const string OFFSET = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";                 //{1} Desde - {2} Hasta

        private const string SELECTCOUNT = @"SELECT COUNT(1) AS rows FROM Category C";

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
            if (!await User.HasRoleAsync(userId, Role.Admin))
                throw new SecurityException();

            if (from.HasValue != length.HasValue)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (length.HasValue && length > 100)
                throw new ArgumentOutOfRangeException(nameof(length));

            string strOrderField;
            if (orderField.HasValue)
                strOrderField = orderField.Value switch
                {
                    Fields.CategoryId => Fields.CategoryId.ToString(),
                    Fields.Name => "B.[Name]",
                    _ => Fields.CategoryId.ToString()
                };
            else
                strOrderField = Fields.CategoryId.ToString();

            using SqlCommand objSqlCmd = new SqlCommand();

            //Filtro
            string strFilter = string.Empty;
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.FreeText))
                {
                    strFilter += " AND CONTAINS(C.*, @FreeText)";
                    objSqlCmd.Parameters.Add("@FreeText", SqlDbType.NVarChar, 1000).Value = SqlCommand.MakeContainsSearchCondition(filter.FreeText, ' ', SqlCommand.FTSOperators.AND, true);
                }
                if (!string.IsNullOrWhiteSpace(strFilter))
                    strFilter = " WHERE " + strFilter.Substring(5);
            }

            string strWithParams = string.Format(SELECT_ALL, strOrderField, !orderAscendant.HasValue || orderAscendant.Value ? "ASC" : "DESC", strFilter);
            objSqlCmd.CommandType = CommandType.Text;

            int recordCount = 0;
            if (from.HasValue)
            {
                strWithParams += string.Format(OFFSET, from, length);

                objSqlCmd.CommandText = SELECTCOUNT + strFilter + strWithParams;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(objSqlCmd.CommandText);
#endif
                DataSet objDS = await objSqlCmd.ExecuteDataSetAsync();
                recordCount = (int)objDS.Tables[0].Rows[0][0];

                return (objDS.Tables[1], recordCount);
            }
            else
            {
                objSqlCmd.CommandText = strWithParams;
                recordCount = -1;
#if DEBUG
                System.Diagnostics.Trace.WriteLine(strWithParams);
#endif
                return (await objSqlCmd.ExecuteDataTableAsync(), recordCount);
            }
        }

        /// <summary>
        /// Elimina una categoría
        /// </summary>
        /// <param name="categoryId">Identificador de la categoría</param>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns><c>true</c> si se pudo eliminar o <c>false</c> en caso que se esté utilizando este registro</returns>
        internal static async Task<bool> DeleteAsync(int userId, int categoryId)
        {
            try
            {
                using var objCmd = new SqlCommand("Category_Delete", CommandType.StoredProcedure);
                objCmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;
                objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                int result = await objCmd.ExecuteReturnInt32Async();

                if (result == Const.SQL_NO_PERMISSION)
                    throw new SecurityException();

                return true;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == Const.SQL_ERROR_DELETE)
                    return false;
                throw;
            }
        }

        /// <summary>
        /// Devuelve los datos de una marca
        /// </summary>
        /// <param name="categoryId">Identificador de la categoría</param>
        /// <param name="userId">Identificador del usuario que realiza la acción</param>
        /// <returns>datos del producto</returns>
        internal static async Task<CategoryModel> GetAsync(int userId, int categoryId)
        {
            using var objCmd = new SqlCommand("Category_Get", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            var result = objCmd.AddReturnParameter();
            using var objDR = await objCmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (result.Value != null && (int)result.Value == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            if (!await objDR.ReadAsync())
                return null;

            return new CategoryModel()
            {
                CategoryId = categoryId,
                Name = objDR.GetString("Name")
            };
        }

        /// <summary>
        /// Graba o actualiza una marca
        /// </summary>
        /// <param name="userId">identificador del usuario que realiza la acción</param>
        /// <param name="data">información a guardar</param>
        /// <returns></returns>
        internal static async Task<bool> SaveAsync(int userId, CategoryModel data)
        {
            using var objCmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure
            };

            if (data.CategoryId.HasValue && data.CategoryId.Value != 0)
            {
                objCmd.CommandText = "Category_Update";
                objCmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = data.CategoryId.Value;
            }
            else
                objCmd.CommandText = "Category_Add";
            objCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = data.Name;
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            int categoryId = await objCmd.ExecuteReturnInt32Async();

            if (categoryId == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            return true;
        }
        #endregion

        /// <summary>
        /// devuelve la lista de marcas de la base de datos
        /// </summary>
        /// <returns></returns>
        internal static async Task<CategoryModel[]> ListAllAsync()
        {
            using var objCmd = new SqlCommand("Category_ListAll", CommandType.StoredProcedure);
            using var objDR = await objCmd.ExecuteReaderAsync();

            var brands = new List<CategoryModel>();

            while (await objDR.ReadAsync())
                brands.Add(
                    new CategoryModel()
                    {
                        CategoryId = objDR.GetInt32("CategoryId"),
                        Name = objDR.GetString("Name"),
                    });
            return brands.ToArray();
        }

    }

}
