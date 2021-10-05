import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent implements OnInit {
  onHover = false;
  product: ProductModel;
  urlFirstImage: string;
  nameImage: string;
  constructor(
    private router: Router,
    private productService: ProductService,
  ) { }

  @Input('product')
  set paramProductId(val: ProductModel) {
    console.log(val);
    if (val !== null) {
      this.product = val;
      this.urlFirstImage = `/api/productImage/${this.product.images[0].productImageId}`;
      this.nameImage = this.product.images[0].filename;
    }
  }

  ngOnInit(): void {
  }

  viewProductDetail(productDetailId: number): void {
    this.router.navigate([`tienda/productos/${productDetailId}`]);
  }
}
