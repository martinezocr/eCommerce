import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { Settings } from '../../app.settings';
import { ProductModel } from '../../models/product.model';
import { CartService } from '../../services/cart.service';
import { ProductService } from '../../services/product.service';


@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  amount: number = 0;
  product: ProductModel;
  urlImage: string;
  constructor(
    private cartService: CartService,
    private productService: ProductService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) { }


  ngOnInit(): void {
    this.route.paramMap
      .subscribe(params => {
        if (params.has('id')) {
          this.productService.get(Number(params.get('id')))
            .then(res => {
              this.product = res;
              this.urlImage = '/api/productImage/' + this.product.images[0].productImageId
            })
            .catch(() => this.snackBar.open(Settings.ERROR_COMM));
        }
      });
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
