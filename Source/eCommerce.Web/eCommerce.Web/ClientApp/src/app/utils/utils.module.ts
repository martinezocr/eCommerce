import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { AuMaskDirective } from './au-mask.directive';
import { SlidePanelComponent } from './slidePanel/slide-panel.component';
import { FileComponent } from './file/file.component';
import { CarouselComponent } from './carousel/carousel.component';

@NgModule({
  declarations: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent
    //AuMaskDirective
  ],
  imports: [
    CommonModule
  ],
  exports: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent
    //AuMaskDirective
  ]
})
export class UtilsModule { }
