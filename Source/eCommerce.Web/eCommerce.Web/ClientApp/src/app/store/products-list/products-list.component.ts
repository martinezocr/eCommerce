import { Component, OnInit, ViewChild, EventEmitter, AfterViewInit, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap, debounceTime, skip } from 'rxjs/operators';
import { Settings } from '../../app.settings';
import { BrandModel } from '../../models/brand..model';
import { CategoryModel } from '../../models/category.model';
import { ProductModel, ProductPublicFields, ProductPublicFilter } from '../../models/product.model';
import { BrandService } from '../../services/brand.service';
import { CategoryService } from '../../services/category.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss']
})
export class ProductsListComponent implements OnInit, AfterViewInit, OnDestroy {
  resultsLength = 0;
  isLoadingResults = true;
  hasError = false;
  data: ProductModel[] = [];
  filter = new ProductPublicFilter();
  draw: number;
  showFilter: boolean;
  refresh = new EventEmitter();
  subs: Subscription;

  //productImageList: ProductImageModel[] = [];
  brands: BrandModel[] = []
  categories: CategoryModel[] = []

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(NgForm) filterForm: NgForm;

  constructor(
    private productService: ProductService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.brandService.listAll()
      .then(data => this.brands = data)
      .catch(() => this.snackBar.open(Settings.ERROR_COMM));

    this.categoryService.listAll()
      .then(data => this.categories = data)
      .catch(() => this.snackBar.open(Settings.ERROR_COMM));

    this.filter.orderBy = ProductPublicFields.relevant
  }

  ngAfterViewInit() {
    this.subs = merge(this.paginator.page, this.filterForm.valueChanges.pipe(skip(1), debounceTime(500)), this.refresh)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          this.draw = (new Date()).getTime();
          return this.productService.publicList(
            this.paginator.pageIndex,
            this.paginator.pageSize,
            this.filter.orderBy,
            this.filter.orderBy === ProductPublicFields.minPrice,
            this.filter,
            this.draw
          );
        }),
        map(data => {
          this.isLoadingResults = false;
          this.hasError = false;
          if (this.draw === data.draw) {
            this.resultsLength = data.recordsCount;
            return data.data;
          }
        }),
        catchError(() => {
          this.hasError = true;
          this.snackBar.open(Settings.ERROR_COMM);
          return observableOf([]);
        })
      ).subscribe(data => this.data = data as ProductModel[]);
  }

  ngOnDestroy(){
    this.subs.unsubscribe();
  }
}
