import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../material.module';
import { SlidePanelComponent } from './slidePanel/slide-panel.component';
import { FileComponent } from './file/file.component';
import { CarouselComponent } from './carousel/carousel.component';

@NgModule({
  declarations: [
    SlidePanelComponent,
    FileComponent,
    CarouselComponent,
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
  ]
})
export class UtilsModule { }
