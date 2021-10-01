//Core de Angular
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

//Módulos
import { StoreRoutingModule } from './store-routing.module';
import { MaterialModule } from '../material.module';
import { UtilsModule } from '../utils/utils.module';

import { ProductItemComponent } from './product-item/product-item.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { ProductsListComponent } from './products-list/products-list.component';
import { StoreComponent } from './store/store.component'
import { HomeComponent } from './home/home.component';
import { CartComponent } from './cart/cart.component'
import { CartItemComponent } from './cart/cart-item/cart-item.component'

@NgModule({
  declarations: [
    ProductItemComponent,
    ProductDetailComponent,
    ProductsListComponent,
    StoreComponent,
    HomeComponent,
    CartComponent,
    CartItemComponent
  ],
  imports: [
    CommonModule,
    StoreRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    UtilsModule
  ],
  providers: [],
  entryComponents: [
    //UserDialog
  ]
})
export class StoreModule { }
