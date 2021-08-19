import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { AuMaskDirective } from './au-mask.directive';
import { SlidePanelComponent } from './slidePanel/slide-panel.component';
import { FileComponent } from './file/file.component';

@NgModule({
  declarations: [
    SlidePanelComponent,
    FileComponent
    //AuMaskDirective
  ],
  imports: [
    CommonModule
  ],
  exports: [
    SlidePanelComponent,
    FileComponent
    //AuMaskDirective
  ]
})
export class UtilsModule { }
