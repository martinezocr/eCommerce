using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace eCommerce.Web
{
    /// <summary>
    /// Modelo para representar las imágenes del producto
    /// </summary>
    public class ProductImageModel
    {
        /// <summary>
        /// identificador del archivo
        /// </summary>
        public Guid? ProductImageId { get; set; }
        /// <summary>
        /// identificador del producto
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Archivo
        /// </summary>
        [JsonIgnore]
        internal FormFile File { get; set; }
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Tipo de archivo
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// Orden en base de datos
        /// </summary>
        public byte Order { get; set; }

        /// <summary>
        /// Devuelve si el archivo es una imagen
        /// </summary>
        /// <returns></returns>
        internal bool IsImage() => MimeType.Split('/')[0].ToLowerInvariant().CompareTo("image") == 0;
    }
}
