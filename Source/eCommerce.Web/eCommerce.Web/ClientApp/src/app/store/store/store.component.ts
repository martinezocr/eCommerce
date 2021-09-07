import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CartModel } from '../../models/cart.model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-store',
  templateUrl: './store.component.html',
  styleUrls: ['./store.component.scss']
})
export class StoreComponent implements OnInit, OnDestroy {
  amountItemsInCart: number;
  subs:Subscription;
  constructor(
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    //obtengo la cantidad de items del carrito para setear el badge
    this.subs = this.cartService.cart$.subscribe(cart=>{
      if(cart?.cartItems?.length)
        this.amountItemsInCart = cart.cartItems.reduce((item1, item2)=> item1+ item2.amount,0);
      else
        this.amountItemsInCart = null;
        
    } );
  }

  ngOnDestroy(){
    this.subs.unsubscribe();
  }
}
