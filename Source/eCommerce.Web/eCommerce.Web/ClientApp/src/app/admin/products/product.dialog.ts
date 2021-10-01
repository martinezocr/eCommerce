import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms'
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductService, SaveResult } from '../../services/product.service';
import { BrandService } from '../../services/brand.service';
import { CategoryService } from '../../services/category.service';
import { Settings } from '../../app.settings';
import { ProductImageModel, ProductModel } from '../../models/product.model';
import { BrandModel } from '../../models/brand..model';
import { CategoryModel } from '../../models/category.model';
import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ViewImageDialog } from '../../utils/view-image/view-image.dialog';

@Component({
  selector: 'app-product',
  templateUrl: './product.dialog.html',
  styleUrls: ['./product.dialog.scss']
})
export class ProductDialog implements OnInit {
  working = true;
  form: FormGroup;

  productImageList: ProductImageModel[] = [];
  brands: BrandModel[] = []
  categories: CategoryModel[] = []

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ProductDialog>,
    @Inject(MAT_DIALOG_DATA) private productId: number,
    private productService: ProductService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,) {
    //Creo el formulario
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required]],
      brandId: ['', [Validators.required]],
      categoryId: ['', [Validators.required]],
      price: ['', [Validators.required, Validators.min(0.1), Validators.pattern(Settings.REGEX_NUMBER_TWO_DECIMALS)]],
      isDiscount: false,
      oldPrice: ['', [Validators.min(0.1), Validators.pattern(Settings.REGEX_NUMBER_TWO_DECIMALS)]],
      isActive: true,
    }, { validator: this.checkOldPrice });
  }

  checkOldPrice(group: FormGroup) { // here we have the 'passwords' group
    let isDiscount: boolean = group.controls.isDiscount.value;
    let oldPrice: number = group.controls.oldPrice.value;
    return isDiscount && (oldPrice === null || oldPrice === undefined) ? { notOldPrice: true } : null;
  }

  ngOnInit() {
    this.brandService.listAll()
      .then(data => this.brands = data)
      .catch(() => this.snackBar.open(Settings.ERROR_COMM));

    this.categoryService.listAll()
      .then(data => this.categories = data)
      .catch(() => this.snackBar.open(Settings.ERROR_COMM));

    //Cargo los datos del usuario
    if (this.productId !== undefined && this.productId !== null)
      this.productService.get(this.productId)
        .then(data => {
          this.form.patchValue(data);
          this.productImageList = data.images;
        })
        .catch(() => {
          this.snackBar.open(Settings.ERROR_COMM);
          this.dialogRef.close(false);
        })
        .finally(() => this.working = false);
    else
      this.working = false;
  }


  onFileInput($event) {
    const inputFile: File = $event.target.files[0];

    const type = inputFile.type.split('/')[0];
    if (type !== 'image') {
      this.snackBar.open('Solo se permiten imágenes');
      return;
    }

    const file = new ProductImageModel();
    file.file = inputFile;
    file.filename = inputFile.name;
    file.mimeType = inputFile.type;
    this.productImageList.push(file);
    $event.target.value = null;
  }

  /**
  * evento drop de los archivos
  * @param event evento
  */
  dropFile(event: CdkDragDrop<ProductImageModel[]>) {
    moveItemInArray(this.productImageList, event.previousIndex, event.currentIndex);
  }

  /**recalcula el orden en que se van a guardar los archivos*/
  recalcOrderFile(files: ProductImageModel[]): ProductImageModel[] {
    files.map((row, index) => {
      row.order = index;
    });
    return files;
  }

  deleteFile(index: number) {
    this.productImageList.splice(index, 1);
  }

  viewFile(index: number) {
    let file: ProductImageModel = this.productImageList[index];
    this.dialog.open(ViewImageDialog, {
      autoFocus: true,
      data: file,
      disableClose: true,
      maxWidth: '90%',
      //width: '1000px',
    });
  }


  /**Cancela la edición */
  cancel() {
    this.dialogRef.close(false);
  }

  save(): void {
    if (this.form.valid) {
      const model = this.form.value as ProductModel;
      model.productId = this.productId;

      model.images = this.recalcOrderFile(this.productImageList);

      if (!model.images || model.images.length < 1) {
        this.snackBar.open('Debe cargar al menos una imagen');
        return;
      }

      this.working = true;
      this.productService.save(model, model.images)
        .then((res) => {
          switch (res) {
            case SaveResult.ok:
              this.dialogRef.close(true);
              break;
            case SaveResult.cantImages:
              this.snackBar.open('Debe cargar al menos una imagen');
              break;
            case SaveResult.OldPriceRequiredForDiscount:
              this.snackBar.open('Si el producto está en oferta, debe cargar el precio anterior');
              break;
          }
        })
        .catch(() => this.snackBar.open(Settings.ERROR_SAVING))
        .finally(() => this.working = false);
    }
  }
}
