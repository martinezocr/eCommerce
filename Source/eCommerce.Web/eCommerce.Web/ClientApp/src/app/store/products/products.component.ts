import { Component, OnInit } from '@angular/core';

export class Filter {
  search: string;
  categoryId: number;
  brandId: number;
  orderBy: number;
}


@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  filters = new Filter();
  constructor() { }

  ngOnInit(): void {
    this.filters.orderBy = 1;
  }

}
