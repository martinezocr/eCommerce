import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ProductModel } from '../models/product.model';
import { Settings } from '../app.settings';
import { CartItemModel, CartModel } from '../models/cart.model';


/**Clase para el manejo de los clientes */
@Injectable({
  providedIn: 'root'
})
export class CartService {
  API: string;

  private cartSource = new BehaviorSubject<CartModel>(null);
  // similar to async pipe
  public readonly cart$: Observable<CartModel> = this.cartSource.asObservable();
  constructor(private http: HttpClient) {
    this.API = Settings.ROOT_CONTROLLERS + 'cart/';    
  }

  /**
   * obtiene los datos del carrito de compras
   * @param cartId identificador del carrito de compras
   */
  getCart(cartId: string):Promise<void> {
    return this.http.get(this.API + cartId)
      .pipe(
        map((cart: CartModel) => {
          this.cartSource.next(cart);
        }))
      .toPromise()
      .then()
      .catch((err)=>console.log(err));
  }

  /**
   * guarda o actualiza el carrito de compras en la base de datos
   * @param cart datos del carrito
   */
  saveCart(cart: CartModel) {
    //this.cartSource.next(cart);
    return this.http.put(this.API, cart)
      .subscribe((response: CartModel) => {
        // This will update the BehaviorSubject withnew value
        this.cartSource.next(response);
      }, (error) => console.log(error));
  }

  /**obtiene el carrito de compras actual */
  getCurrentCartValue(): CartModel {
    return this.cartSource.value;
  }

  /**
   * agrega un item/producto al carrito de compras
   * @param item item a agregar
   * @param amount cantidad a agregar
   */
  addItemToCart(item: ProductModel, amount = 1) {
    if(amount < 1)
      return;
    const itemToAdd = this.product2CartItem(item)
    const cart = this.getCurrentCartValue() ?? this.createCart();
    cart.cartItems = this.addOrUpdateItem(cart.cartItems, itemToAdd, amount);
    this.saveCart(cart);
  }

  /**
   * incrementa la cantidad de un item/producto
   * @param item item seleccionado
   */
  incrementItemQuantity(item: CartItemModel) {
    const cart = this.getCurrentCartValue();
    const foundItemIndex = cart.cartItems.findIndex(x => x.cartItemId === item.cartItemId);
    cart.cartItems[foundItemIndex].amount++;
    this.saveCart(cart);
  }

/**
 * decrementa la cantidad de un item/producto
 * @param item item seleccionado
 */
  decrementItemQuantity(item: CartItemModel) {
    const cart = this.getCurrentCartValue();
    const foundItemIndex = cart.cartItems.findIndex(x => x.cartItemId === item.cartItemId);
    if (cart.cartItems[foundItemIndex].amount > 1) {
      cart.cartItems[foundItemIndex].amount--;
      this.saveCart(cart);
    } else {
      this.removeItemFromCart(item);
    }
  }

  /**
   * elimina un item del carrito, si el carrito no tiene productos, lo elimina
   * @param cartItem item a eliminar
   */
  removeItemFromCart(cartItem: CartItemModel) {
    const cart = this.getCurrentCartValue();
    if (cart.cartItems.some(x => x.cartItemId === cartItem.cartItemId)) {
      cart.cartItems = cart.cartItems.filter(x => x.cartItemId !== cartItem.cartItemId)
      if (cart.cartItems.length > 0) {
        this.saveCart(cart);
      } else {
        this.deleteCart(cart.cartId);
      }
    }
  }

  /**
   * elimina el carrito
   * @param cartId identificador del carrito
   */
  deleteCart(cartId: string) {
    this.cartSource.next(null);
      localStorage.removeItem('cart_id');
    // return this.http.delete(this.API + cartId).subscribe(() => {
    //   this.cartSource.next(null);
    //   localStorage.removeItem('cart_id');
    // }, error => {
    //   console.log(error);
    // });
  }

  /**
   * agrega o actualiza un item del carrito
   * @param items items del carrito
   * @param itemToAdd item a agregar o actualizar cantidad
   * @param amount cantidad a actualizar
   */
  private addOrUpdateItem(items: CartItemModel[], itemToAdd: CartItemModel, amount: number): CartItemModel[] {
    const index = items.findIndex(i => i.cartItemId === itemToAdd.cartItemId);
    if (index === -1) {
      itemToAdd.amount = amount;
      itemToAdd.order = items.length + 1;
      items.push(itemToAdd);
    } else {
      items[index].amount += amount;
    }
    return items;
  }

  /**crea un nuevo carrito de compras */
  private createCart(): CartModel {
    const cart = new CartModel();
    cart.cartId = 'msdl-122a-asd1-asda-a5s4-kams';
    localStorage.setItem('cart_id', cart.cartId);
    return cart;
  }

  private product2CartItem(product:ProductModel):CartItemModel{
    let cartItem = new CartItemModel();
    cartItem.description = product.description;
    cartItem.title = product.title;
    cartItem.price = product.price;
    cartItem.productId = product.productId
    return cartItem;
  }

}
