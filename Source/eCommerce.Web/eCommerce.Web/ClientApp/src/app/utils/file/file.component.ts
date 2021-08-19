import { Component, Input, ViewEncapsulation } from '@angular/core';
import { FileModel } from '../../models/file.model';
import { Settings } from '../../app.settings';

@Component({
  encapsulation: ViewEncapsulation.ShadowDom,
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.scss']
})
export class FileComponent {

  @Input() file: FileModel;

  url = () => Settings.ROOT_CONTROLLERS + 'file/' + this.file.fileId;
}
