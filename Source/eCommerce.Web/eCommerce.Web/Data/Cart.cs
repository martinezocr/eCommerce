using CustomSqlClient.Net.Core;
using eCommerce.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace eCommerce.Web.Data
{
    /// <summary>
    /// Clase para el manejo de la información del carrito de compras
    /// </summary>
    public static class Cart
    {
        /// <summary>
        /// guarda o actualiza los datos del carrito de compras
        /// </summary>
        /// <param name="data">datos del carrito</param>
        /// <returns></returns>
        internal static async Task<Guid> SaveAsync(CartModel data)
        {
            using var cmd = new SqlCommand() { CommandType = CommandType.StoredProcedure };
            await cmd.BeginTransactionAsync();

            try
            {
                if (!data.CartId.HasValue)
                {
                    cmd.CommandText = "Cart_Save";
                    cmd.Parameters.Add("@ExpirationDay", SqlDbType.Date).Value = DateTime.Now.AddDays(Const.CART_EXPIRATION_DAYS);
                    var paramCartId = cmd.Parameters.Add("@OutputCartId", SqlDbType.UniqueIdentifier);
                    paramCartId.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    data.CartId = (Guid)paramCartId.Value;
                    cmd.Parameters.Clear();
                }
                else
                {
                    cmd.CommandText = "CartItem_Clean";
                    cmd.Parameters.Add("@CartId", SqlDbType.UniqueIdentifier).Value = data.CartId.Value;
                    if (data.CartItems?.Length > 0)
                        cmd.Parameters.Add("@ExceptCartItemIds", SqlDbType.NVarChar, 1000).Value = string.Join(",", data.CartItems.Select(c => c.CartItemId));
                    await cmd.ExecuteNonQueryAsync();

                }

                cmd.CommandText = "CartItem_Save";
                if (data.CartItems?.Length > 0)
                    foreach (var item in data.CartItems)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CartId", SqlDbType.UniqueIdentifier).Value = data.CartId.Value;
                        if (item.CartItemId.HasValue)
                            cmd.Parameters.Add("@CartItemId", SqlDbType.Int).Value = item.CartItemId.Value;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                        cmd.Parameters.Add("@Amount", SqlDbType.TinyInt).Value = item.Amount;
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = item.Title;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = item.Description;
                        cmd.Parameters.Add("@Order", SqlDbType.TinyInt).Value = item.Order;
                        await cmd.ExecuteNonQueryAsync();
                    }

                await cmd.CommitAsync();
                return data.CartId.Value;
            }
            catch (Exception ex)
            {
                await cmd.RollbackAsync();
                throw;
            }
        }
        /// <summary>
        /// elimina el carrito de compras
        /// </summary>
        /// <param name="cartId">identificador del carrito de compras</param>
        /// <returns></returns>
        internal static async Task<bool> DeleteAsync(Guid cartId)
        {
            using var cmd = new SqlCommand()
            {
                CommandText = "Cart_Delete",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("CartId", SqlDbType.UniqueIdentifier).Value = cartId;
            return await cmd.ExecuteReturnInt32Async() > 0;
        }

        /// <summary>
        /// obtiene los datos de un carrito de compras
        /// </summary>
        /// <param name="cartId">identificador del carrito de compas</param>
        /// <returns></returns>
        internal static async Task<CartModel> GetByIdAsync(Guid cartId)
        {
            using var cmd = new SqlCommand()
            {
                CommandText = "Cart_Get",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CartId", SqlDbType.UniqueIdentifier).Value = cartId;

            var cartItems = new List<CartItemModel>();
            using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                var cartItem = new CartItemModel()
                {
                    CartId = cartId,
                    CartItemId = reader.GetInt32("CartItemId"),
                    ProductId = reader.GetInt32("ProductId"),
                    Amount = reader.GetByte("Amount"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    Order = reader.GetByte("Order"),
                    Price = (decimal)1500.50
                };
                cartItems.Add(cartItem);
            }
            return new CartModel()
            {
                CartId = cartId,
                CartItems = cartItems.ToArray()
            };

        }
    }
}