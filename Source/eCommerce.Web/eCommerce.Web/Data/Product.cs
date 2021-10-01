using CustomSqlClient.Net.Core;
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
    /// clase para el manejo de datos de los productos
    /// </summary>
    public static class Product
    {
        #region Consulta Public

        /// <summary>
        /// Campos de la consulta
        /// </summary>
        public enum PublicFields
        {
            ProductId = 0,
            Relevant = 1,
            MinPrice = 2,
            MaxPrice = 3
        }

        /// <summary>
        /// Clase para definir los valores de los filtros
        /// </summary>
        [Serializable]
        public class PublicFilter
        {
            /// <summary>
            /// filtro por marca
            /// </summary>
            public int? BrandId { get; set; }
            /// <summary>
            /// filtro por categoría
            /// </summary>
            public int? CategoryId { get; set; }
            /// <summary>
            /// Búsqueda por texto libre
            /// </summary>
            public string FreeText { get; set; }
        }

        private const string PUBLIC_SELECT_ALL =
            ";SELECT E.QuestionId, E.CategoryId, " +
            " case when @Lang = 'es' then E.[QuestionEs] else E.[QuestionCa] end as Question," +
            " E.IsActive, E.QuestionTypeId" +
            " FROM [Question] AS E" +
            " {2}" +
            " ORDER BY {0} {1}";

        private const string PUBLIC_OFFSET = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";                 //{1} Desde - {2} Hasta

        private const string PUBLIC_SELECTCOUNT = "SELECT COUNT(1) AS rows FROM [Question] AS E";

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
        public static async Task<(DataTable, int)> PublicListAsync(int userId, PublicFields? orderField, bool? orderAscendant, PublicFilter filter, int? from, int? length)
        {
            if (from.HasValue != length.HasValue)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (length.HasValue && length > 100)
                throw new ArgumentOutOfRangeException(nameof(length));

            string strOrderField;
            if (orderField.HasValue)
                strOrderField = orderField.Value switch
                {
                    PublicFields.Relevant => "",
                    PublicFields.MinPrice => "",
                    PublicFields.MaxPrice => "",
                    _ => PublicFields.ProductId.ToString(),
                };
            else
                strOrderField = PublicFields.ProductId.ToString();

            using SqlCommand objSqlCmd = new SqlCommand();

            //Filtro
            string strFilter = string.Empty;
            if (filter != null)
            {
                if (filter.CategoryId.HasValue)
                {
                    strFilter += " AND E.CategoryId = @CategoryId ";
                    objSqlCmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = filter.CategoryId;
                }

                if (filter.BrandId.HasValue)
                {
                    strFilter += " AND E.BrandId = @BrandId ";
                    objSqlCmd.Parameters.Add("@BrandId", SqlDbType.Int).Value = (byte)filter.BrandId;
                }

                if (!string.IsNullOrWhiteSpace(filter.FreeText))
                {
                    strFilter += " AND CONTAINS(E.*, @FreeText)";
                    objSqlCmd.Parameters.Add("@FreeText", SqlDbType.NVarChar, 1000).Value = SqlCommand.MakeContainsSearchCondition(filter.FreeText, ' ', SqlCommand.FTSOperators.AND, true);
                }
                if (!string.IsNullOrWhiteSpace(strFilter))
                    strFilter = " WHERE " + strFilter.Substring(5);
            }

            string strWithParams = string.Format(PUBLIC_SELECT_ALL, strOrderField, !orderAscendant.HasValue || orderAscendant.Value ? "ASC" : "DESC", strFilter);
            objSqlCmd.CommandType = CommandType.Text;
            int recordCount = 0;
            if (from.HasValue)
            {
                strWithParams += string.Format(PUBLIC_OFFSET, from, length);

                objSqlCmd.CommandText = PUBLIC_SELECTCOUNT + strFilter + strWithParams;
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

        #endregion

        #region Administracion
        public enum SaveResult : int
        {
            /// <summary>
            /// todo bien
            /// </summary>
            Ok = 0,
            /// <summary>
            /// no contiene imágenes
            /// </summary>
            CantImages = -1,
            /// <summary>
            /// Precio anterior requerido cuando hay un descuento
            /// </summary>
            OldPriceRequiredForDiscount = -2
        }

        /// <summary>
        /// Campos de la consulta
        /// </summary>
        public enum Fields
        {
            ProductId = 0,
            Brand = 1,
            Category = 2,
            Title = 3,
            Price = 4,
            IsDiscount = 5,
            IsActive = 6,
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
            @" ;SELECT P.ProductId, B.[Name] as Brand, C.[Name] as Category, P.Title, P.IsDiscount, P.Price,P.IsActive
                FROM Product P INNER JOIN Brand B ON P.BrandId = B.BrandId
                INNER JOIN Category C ON P.CategoryId = C.CategoryId 
                {2} 
                ORDER BY {0} {1} ";

        private const string OFFSET = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";                 //{1} Desde - {2} Hasta

        private const string SELECTCOUNT = @"SELECT COUNT(1) AS rows FROM [Product] AS P INNER JOIN Brand B ON P.BrandId = B.BrandId
                                                INNER JOIN Category C ON P.CategoryId = C.CategoryId";

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
                    Fields.ProductId => Fields.ProductId.ToString(),
                    Fields.Brand => "B.[Name]",
                    Fields.Category => "C.[Name]",
                    Fields.Title => "P.Title",
                    Fields.Price => "P.Price",
                    Fields.IsDiscount => "P.IsDiscount",
                    Fields.IsActive => "P.IsActive",
                    _ => Fields.ProductId.ToString(),
                };
            else
                strOrderField = Fields.ProductId.ToString();

            using SqlCommand objSqlCmd = new SqlCommand();

            //Filtro
            string strFilter = string.Empty;
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.FreeText))
                {
                    strFilter += " AND CONTAINS(P.*, @FreeText)";
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
        /// Elimina una pregunta
        /// </summary>
        /// <param name="productId">Identificador del producto que se va a eliminar</param>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="folderUpload">Path de la carpeta donde se encuentra el archivo a eliminar</param>
        /// <returns><c>true</c> si se pudo eliminar o <c>false</c> en caso que se esté utilizando este registro</returns>
        internal static async Task<bool> DeleteAsync(int userId, int productId, string folderUpload)
        {
            try
            {
                //obtengo los id de los archivos antes de eliminarlos
                Guid[] fileIds = (await ProductImage.ListByProductIdAsync(productId)).Select(p => p.ProductImageId.Value).ToArray();

                using var objCmd = new SqlCommand("Product_Delete", CommandType.StoredProcedure);
                objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                objCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                int result = await objCmd.ExecuteReturnInt32Async();

                if (result == Const.SQL_NO_PERMISSION)
                    throw new SecurityException();
                //elimino los archivos
                foreach (Guid fileId in fileIds)
                    ProductImage.Delete(Path.Combine(folderUpload, fileId.ToString()));

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
        /// Devuelve los datos de un producto
        /// </summary>
        /// <param name="productId">Identificador del producto</param>
        /// <param name="userId">Identificador del usuario que realiza la acción</param>
        /// <returns>datos del producto</returns>
        internal static async Task<ProductModel> GetAsync(int userId, int productId)
        {
            using var objCmd = new SqlCommand("Product_Get", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            objCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
            var result = objCmd.AddReturnParameter();
            using var objDR = await objCmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (result.Value != null && (int)result.Value == Const.SQL_NO_PERMISSION)
                throw new SecurityException();

            if (!await objDR.ReadAsync())
                return null;

            return new ProductModel()
            {
                ProductId = objDR.GetInt32("ProductId"),
                BrandId = objDR.GetInt32("BrandId"),
                CategoryId = objDR.GetInt32("CategoryId"),
                Title = objDR.GetString("Title"),
                Description = objDR.GetString("Description"),
                IsDiscount = objDR.GetBoolean("IsDiscount"),
                Price = objDR.GetDecimal("Price"),
                OldPrice = await objDR.IsDBNullAsync("OldPrice") ? (decimal?)null : objDR.GetDecimal("OldPrice"),
                IsActive = objDR.GetBoolean("IsActive"),
                Images = await ProductImage.ListByProductIdAsync(productId)
            };
        }

        /// <summary>
        /// Graba o actualiza un producto
        /// </summary>
        /// <param name="userId">identificador del usuario que realiza la acción</param>
        /// <param name="data">información a guardar</param>
        /// <returns></returns>
        internal static async Task<SaveResult> SaveAsync(int userId, ProductModel data, string folderUpload)
        {
            if (data.IsDiscount && !data.OldPrice.HasValue)
                return SaveResult.OldPriceRequiredForDiscount;

            if (data.Images?.Length < 1)
                return SaveResult.CantImages;

            using var objCmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure
            };
            await objCmd.BeginTransactionAsync();

            var fileInserted = new List<string>();
            try
            {
                if (data.ProductId.HasValue && data.ProductId.Value != 0)
                {
                    objCmd.CommandText = "Product_Update";
                    objCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = data.ProductId.Value;
                }
                else
                    objCmd.CommandText = "Product_Add";
                objCmd.Parameters.Add("@BrandId", SqlDbType.Int).Value = data.BrandId;
                objCmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = data.CategoryId;
                objCmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = data.Title;
                objCmd.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = data.Description;
                objCmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = data.Price;
                objCmd.Parameters.Add("@IsDiscount", SqlDbType.Bit).Value = data.IsDiscount;
                if (data.IsDiscount)
                    objCmd.Parameters.Add("@OldPrice", SqlDbType.Decimal).Value = data.OldPrice.Value;
                objCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = data.IsActive;

                objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                int productId = await objCmd.ExecuteReturnInt32Async();

                if (productId == Const.SQL_NO_PERMISSION)
                    throw new SecurityException();

                /*guardo los archivos para preguntas para cualquier tipo de pregunta*/
                ///limpio los archivos
                if (data.ProductId.HasValue)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "ProductImage_Clear";
                    objCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = data.ProductId.Value;
                    objCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    var paramReturn = objCmd.AddReturnParameter();
                    if (data.Images?.Length > 0 && data.Images?.Where(p => p.ProductImageId.HasValue).Count() > 0)
                        objCmd.Parameters.Add("@ExceptProductImageIds", SqlDbType.NVarChar, 1000).Value = string.Join(",", data.Images.Where(p => p.ProductImageId.HasValue).Select(p => p.ProductImageId));

                    Guid[] fileIds = await objCmd.ExecuteArrayGuidAsync();

                    if (paramReturn.Value != null && (int)paramReturn.Value == Const.SQL_NO_PERMISSION)
                        throw new SecurityException();

                    //elimino los archivos
                    foreach (Guid fileId in fileIds)
                        File.Delete(Path.Combine(folderUpload, fileId.ToString()));
                }
                ///guardo los archvios
                if (data.Images?.Length > 0)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "ProductImage_Add";
                    var paramProductImageId = objCmd.Parameters.Add("ProductImageId", SqlDbType.UniqueIdentifier);
                    var paramProductId = objCmd.Parameters.Add("ProductId", SqlDbType.Int);
                    var paramFilename = objCmd.Parameters.Add("Filename", SqlDbType.NVarChar, 255);
                    var paramMimeType = objCmd.Parameters.Add("MimeType", SqlDbType.NVarChar, 255);
                    var paramOrder = objCmd.Parameters.Add("Order", SqlDbType.TinyInt);
                    var paramUser = objCmd.Parameters.Add("@UserId", SqlDbType.Int);
                    var paramOutputProductImageId = objCmd.Parameters.Add("@OutProductImageId", SqlDbType.UniqueIdentifier);
                    paramOutputProductImageId.Direction = ParameterDirection.Output;

                    foreach (var item in data.Images)
                    {
                        paramProductImageId.Value = item.ProductImageId.HasValue ? item.ProductImageId.Value : DBNull.Value;
                        paramProductId.Value = productId;
                        paramOrder.Value = item.Order;
                        paramUser.Value = userId;

                        if (item.File?.Length > 0)
                        {
                            paramFilename.Value = item.File.FileName;
                            paramMimeType.Value = item.File.ContentType;
                        }

                        if (await objCmd.ExecuteReturnInt32Async() == Const.SQL_NO_PERMISSION)
                            throw new SecurityException();

                        if (paramOutputProductImageId.Value != DBNull.Value) //si guardo la data de la imagen en Bd, grabo la imagen en disco
                            using (var stream = System.IO.File.Create(Path.Combine(folderUpload, paramOutputProductImageId.Value.ToString())))
                            {
                                await item.File.CopyToAsync(stream);
                                fileInserted.Add(paramOutputProductImageId.Value.ToString());
                            }
                    }
                }
                /*fin del guardado de archivos*/

                await objCmd.CommitAsync();
                return SaveResult.Ok;
            }
            catch
            {
                await objCmd.RollbackAsync();
                //elimino los archivos guardados durante la transacción
                foreach (string fileId in fileInserted)
                {
                    string path = Path.Combine(folderUpload, fileId);
                    try
                    {
                        Data.ProductImage.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                    }
                }
                throw;
            }
        }

        #endregion
    }
}
