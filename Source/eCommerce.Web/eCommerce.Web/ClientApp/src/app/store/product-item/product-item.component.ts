import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent implements OnInit {
  onHover = false;
  constructor(
    private router: Router
  ) { }

  ngOnInit(): void {
  }

  viewProductDetail(productDetailId: number): void {
    this.router.navigate([`tienda/productos/${productDetailId}`]);
  }
}
