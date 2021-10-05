import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../material.module';
import { SlidePanelComponent } from './slidePanel/slide-panel.component';
import { FileComponent } from './file/file.component';
import { CarouselComponent } from './carousel/carousel.component';
import { ViewImageDialog } from './view-image/view-image.dialog';
import { CardLoadingComponent } from './card-loading/card-loading.component';

@NgModule({
  declarations: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent,
    ViewImageDialog,
    CardLoadingComponent,
  ],
  imports: [
    CommonModule,
    MaterialModule
  ],
  exports: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent,
    CardLoadingComponent
    //AuMaskDirective
  ],
  entryComponents: [
    ViewImageDialog
  ]
})
export class UtilsModule { }
