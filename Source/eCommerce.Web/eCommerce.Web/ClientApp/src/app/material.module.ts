import { NgModule } from '@angular/core';
import { Settings } from './app.settings';

//Material
import { MatButtonModule } from '@angular/material/button';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatPaginatorModule, MatPaginatorIntl, MAT_PAGINATOR_DEFAULT_OPTIONS } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { MAT_TOOLTIP_DEFAULT_OPTIONS } from '@angular/material/tooltip';
import { CdkTableModule } from '@angular/cdk/table';
import { FlexLayoutModule } from '@angular/flex-layout';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatStepperModule } from '@angular/material/stepper';
import { MatBadgeModule } from '@angular/material/badge'

//Servicios
import { PaginatorIntl } from './services/paginatorIntl.service';


@NgModule({
  exports: [
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    MatTableModule,
    MatPaginatorModule,
    MatMenuModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatInputModule,
    MatTooltipModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatCardModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    MatCheckboxModule,
    MatTabsModule,
    MatButtonToggleModule,
    CdkTableModule,
    FlexLayoutModule,
    DragDropModule,
    MatStepperModule,
    MatBadgeModule
  ],
  providers: [
    { provide: MatPaginatorIntl, useClass: PaginatorIntl },
    { provide: MAT_DATE_LOCALE, useValue: Settings.DATE_LOCALE },
    { provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: Settings.TOOLTIP_DEFAULTS },
    { provide: MAT_SNACK_BAR_DEFAULT_OPTIONS, useValue: Settings.SNACKBAR_DEFAULTS },
    { provide: MAT_PAGINATOR_DEFAULT_OPTIONS, useValue: Settings.PAGINATOR_DEFAULTS }
  ]
})
export class MaterialModule { }
