import { Component, AfterViewInit, ViewChild, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { UserModel, UserFields, UserFilter } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap, debounceTime, skip } from 'rxjs/operators';
import { NgForm } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Settings } from '../../app.settings';
import { ConfirmDialog } from '../../dialogs/confirm.dialog';
import { UserDialog } from './user.dialog';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, AfterViewInit, OnDestroy {

  displayedColumns: string[] = ['username', 'firstName','lastName', 'isActive', 'tools'];

  resultsLength = 0;
  isLoadingResults = true;
  hasError = false;
  data: UserModel[] = [];
  filter = new UserFilter();
  draw: number;
  showFilter: boolean;
  refresh = new EventEmitter();
  subs: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(NgForm) filterForm: NgForm;

  constructor(
    private userService: UserService,
    private titleService: Title,
    private dialog: MatDialog,
    private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.titleService.setTitle(Settings.TITLE + " - Usuarios");
  }

  ngAfterViewInit() {
    this.subs = merge(this.sort.sortChange, this.paginator.page, this.filterForm.valueChanges.pipe(skip(1), debounceTime(500)), this.refresh)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          this.draw = (new Date()).getTime();
          return this.userService.list(
            this.paginator.pageIndex,
            this.paginator.pageSize,
            this.sort && this.sort.direction ? UserFields[this.sort.active] : null,
            this.sort.direction === "asc",
            this.filter,
            this.draw
          );
        }),
        map(data => {
          this.isLoadingResults = false;
          this.hasError = false;
          if (this.draw === data.draw) {
            this.resultsLength = data.recordsCount;
            return data.data;
          }
        }),
        catchError(() => {
          this.hasError = true;
          this.snackBar.open(Settings.ERROR_COMM);
          return observableOf([]);
        })
      ).subscribe(data => this.data = data as UserModel[]);
  }

  delete({ userId }: UserModel) {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      autoFocus: true,
      data: { message: '¿Está seguro de eliminar el usuario?' }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed)
        this.userService.delete(userId)
          .then(() => {
            this.snackBar.open('Se ha eliminado el usuario');
            if (this.data.length == 1 && this.paginator.pageIndex != 0)
              this.paginator.firstPage();
            else
              this.refresh.emit();
          })
          .catch(() => this.snackBar.open(Settings.ERROR_COMM));
    });
  }

  edit(userId: number = null) {
    const dialogRef = this.dialog.open(UserDialog, {
      autoFocus: true,
      data: userId,
      disableClose: true,
      maxWidth: 581
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result)
        this.refresh.emit();
    });
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  download() {
    window.location.href = "/api/user/download-excel";
  }
}
