import { Component, Input, ViewEncapsulation } from '@angular/core';
import { ProductImageModel } from '../../models/product-image.model';
import { Settings } from '../../app.settings';

@Component({
  encapsulation: ViewEncapsulation.ShadowDom,
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.scss']
})
export class FileComponent {

  @Input() file: ProductImageModel;

  url = () => Settings.ROOT_CONTROLLERS + 'productimage/' + this.file.productImageId;
}
