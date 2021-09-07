import { AfterViewInit, Component, Input, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ProductModel } from '../../models/product.model';
import { CartItemModel, CartModel } from '../../models/cart.model';
import { CartItemComponent } from './cart-item/cart-item.component';
import { CartService } from '../../services/cart.service';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit, OnDestroy,AfterViewInit {
  subtotal: number = 0;
  totalDiscount: number = 0;
  total: number = 0;
  showCart: boolean = false;
  subs:Subscription;

  cartItems: CartItemModel[]=[];
  //@ViewChildren('cartItem') cartItem: QueryList<CartItemComponent>;

  constructor(
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.subs = this.cartService.cart$.subscribe(cart=> {
      this.showCart = cart?.cartItems?.length > 0
      if(this.showCart){
        this.cartItems = cart.cartItems;
        this.subtotal = this.cartItems.reduce((sub, item)=> sub + item.amount *item.price,0);
        this.total = this.subtotal - this.totalDiscount;
      }
    });
  }

  ngAfterViewInit():void{
    
  }

  ngOnDestroy():void{
    this.subs.unsubscribe();
  }

  //setTotal() {
  //  this.subtotal = 0;
  //  this.cartItems.forEach(item => {
  //    this.subtotal += item.subtotal;
  //  });

  //  this.total = this.subtotal - this.totalDiscount;
  //}

}
