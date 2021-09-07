import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ProductModel } from '../../../models/product.model'
import { CartItemModel, CartModel } from '../../../models/cart.model'
import { CartService } from '../../../services/cart.service'
import { Observable } from 'rxjs';

@Component({
  selector: 'app-cart-item',
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss']
})
export class CartItemComponent implements OnInit {
  cartItem: CartItemModel;

  @Input('cart-item')
  set productItem(val: CartItemModel) {
    if (val != null)
      this.cartItem = val;
  }

  constructor(
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    
  }

  decrement(): void {
    this.cartService.decrementItemQuantity(this.cartItem);
  }
  increment(): void {
    this.cartService.incrementItemQuantity(this.cartItem);
  }
  remove(): void {
    this.cartService.removeItemFromCart(this.cartItem);
  }

}
