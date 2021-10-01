import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductImageModel } from '../../models/product-image.model';

@Component({
  selector: 'app-view-image',
  templateUrl: './view-image.dialog.html',
  styleUrls: ['./view-image.dialog.scss']
})
export class ViewImageDialog implements OnInit {

  file: ProductImageModel;
  constructor(
    public dialogRef: MatDialogRef<ViewImageDialog>,
    @Inject(MAT_DIALOG_DATA) private data: ProductImageModel) {
  }

  ngOnInit() {
    this.file = this.data;
  }

  cancel() {
    this.dialogRef.close(false);
  }

}
