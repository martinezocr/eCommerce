/**Entidad del carrito de compras */
export class CartModel {
  cartId: string;
  cartItems: CartItemModel[] = [];
}

/**items del carrito de compras */
export class CartItemModel {
  cartItemId: string;
  productId: number;
  amount: number;
  title: string;
  description: string;
  price: number;
  order: number;
}
