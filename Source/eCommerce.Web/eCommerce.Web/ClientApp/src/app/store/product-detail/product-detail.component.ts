import { Component, Input, OnInit } from '@angular/core';
import { ProductModel } from '../../models/product.model';
import { CartService } from '../../services/cart.service';


@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  amount: number = 0;
  product: ProductModel;

  constructor(
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.product = new ProductModel();
    this.product.name = 'Pelota de futbol N5 para chicos'
    this.product.price = 1545.50;
    this.product.description = "descripción del producto";
    this.product.productId = 1;
  }

  increment(): void {
    this.amount++;
  }
  decrement(): void {
    if (this.amount > 0)
      this.amount--;
  }
  addToCart(): void {
    this.cartService.addItemToCart(this.product, this.amount);
  }

}
