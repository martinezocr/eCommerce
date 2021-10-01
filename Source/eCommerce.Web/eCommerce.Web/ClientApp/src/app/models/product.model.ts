/**Entidad de las categorías */
export class ProductModel {
  productId: number;
  categoryId: number;
  brandId: number;
  title: string;
  description: string;
  price: number;
  oldPrice: number;
  isDiscount: boolean
  isActive: boolean
  images: ProductImageModel[];
}

export class ProductImageModel {
  productImageId: string;
  productId: number;
  order: number;
  file: File;
  filename: string;
  mimeType: string;
}

export enum ProductFields {
  productId = 0,
  brand = 1,
  category = 2,
  title = 3,
  price = 4,
  isDiscount = 5,
  isActive = 6
}

export class ProductFilter {
  freeText: string;
}
