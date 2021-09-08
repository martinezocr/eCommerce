/**Entidad de las categorías */
export class ProductModel {
  productId: number;
  categoryId: number;
  brandId: number;
  title: string;
  description: string;
  price: number;
  oldPrice: number;
  images: ProductImagesModel[];
}

export class ProductImagesModel {
  productImageId: string;
  order: number;
  url: string;
  name: string;
}
