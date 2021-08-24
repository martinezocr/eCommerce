import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CartComponent } from './cart/cart.component';
import { HomeComponent } from './home/home.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { ProductsComponent } from './products/products.component'
const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'tienda/productos',
    component: ProductsComponent,
  },
  {
    path: 'tienda/productos/:id',
    component: ProductDetailComponent,
  },
  {
    path: 'tienda/carrito',
    component: CartComponent,
  },
  //{
  //  path: 'torneos',
  //  loadChildren: () => import('../crud/tournaments/tournaments.module').then(mod => mod.TournamentsModule),
  //  data: { roles: [UserRole.Admin], login: '/admin/ingreso' },
  //  canActivate: [RoleGuard]
  //},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StoreRoutingModule { }
