using eCommerce.Web.Models;
using CustomSqlClient.Net.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Threading.Tasks;

namespace eCommerce.Web.Data
{
    /// <summary>
    /// Clase para consultar/editar los archivos asociados a las preguntas
    /// </summary>
    public static class ProductImage
    {
        /// <summary>
        /// obtiene la lista con los archivos asociados a un producto
        /// </summary>
        /// <param name="productId">Identificador de la pregunta</param>
        /// <returns>Lista con los archivos asociados a la pregunta</returns>
        internal static async Task<ProductImageModel[]> ListByProductIdAsync(int productId)
        {
            using var objCmd = new SqlCommand("ProductImage_ListByProductId", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
            using var objDR = await objCmd.ExecuteReaderAsync();

            var files = new List<ProductImageModel>();

            while (await objDR.ReadAsync())
                files.Add(
                    new ProductImageModel()
                    {
                        ProductImageId = objDR.GetGuid("ProductImageId"),
                        ProductId = objDR.GetInt32("ProductId"),
                        Filename = objDR.GetString("Filename"),
                        MimeType = objDR.GetString("MimeType"),
                        Order = objDR.GetByte("Order"),
                    });

            return files.ToArray();
        }

        /// <summary>
        /// Devuelve los datos de un archivo
        /// </summary>
        /// <param name="productImageId">Identificador del archivo</param>
        /// <returns>Datos del archivo</returns>
        internal static async Task<ProductImageModel> Get(Guid productImageId)
        {
            using var objCmd = new SqlCommand("ProductImage_Get", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@ProductImageId", SqlDbType.UniqueIdentifier).Value = productImageId;
            using var objDR = await objCmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await objDR.ReadAsync())
                return null;

            return new ProductImageModel()
            {
                ProductImageId = productImageId,
                ProductId = objDR.GetInt32("ProductId"),
                Filename = objDR.GetString("Filename"),
                MimeType = objDR.GetString("MimeType")
            };
        }

        /// <summary>
        /// Devuelve los datos de la primer imagen del producto
        /// </summary>
        /// <param name="productId">Identificador del producto</param>
        /// <returns>Datos del archivo</returns>
        internal static async Task<ProductImageModel> GetFirst(int productId)
        {
            using var objCmd = new SqlCommand("ProductImage_GetFirst", CommandType.StoredProcedure);
            objCmd.Parameters.Add("@ProductImageId", SqlDbType.UniqueIdentifier).Value = productId;
            using var objDR = await objCmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await objDR.ReadAsync())
                return null;

            return new ProductImageModel()
            {
                ProductImageId = objDR.GetGuid("ProductImageId"),
                ProductId = productId,
                Filename = objDR.GetString("Filename"),
                MimeType = objDR.GetString("MimeType")
            };
        }

        /// <summary>
        /// método para eliminar, si es que existe, un archivo
        /// </summary>
        /// <param name="filepath">path completo del archivo</param>
        internal static void Delete(string filepath)
        {
            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);
        }
    }
}