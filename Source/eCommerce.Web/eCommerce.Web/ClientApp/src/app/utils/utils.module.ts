import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../material.module';
import { SlidePanelComponent } from './slidePanel/slide-panel.component';
import { FileComponent } from './file/file.component';
import { CarouselComponent } from './carousel/carousel.component';
import { ViewImageDialog } from './view-image/view-image.dialog';

@NgModule({
  declarations: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent,
    ViewImageDialog,
  ],
  imports: [
    CommonModule,
    MaterialModule
  ],
  exports: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent
    //AuMaskDirective
  ],
  entryComponents: [
    ViewImageDialog
  ]
})
export class UtilsModule { }
