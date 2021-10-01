import { Component, OnInit } from '@angular/core';

export class Filter {
  search: string;
  categoryId: number;
  brandId: number;
  orderBy: number;
}


@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss']
})
export class ProductsListComponent implements OnInit {

  filters = new Filter();
  constructor() { }

  ngOnInit(): void {
    this.filters.orderBy = 1;
  }

}
