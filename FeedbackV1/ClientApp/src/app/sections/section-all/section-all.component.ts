import { Pagination, PaginatedResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DialogService } from '../../_services/dialog.service';

@Component({
  selector: 'app-section-all',
  templateUrl: './section-all.component.html',
  styleUrls: ['./section-all.component.css'],
})
export class SectionAllComponent implements OnInit {

  filter: string;
  users: User[];
  userTeam: User[];
  id: any;
  modalRef: BsModalRef;
  departamente = ['Nothing', 'Suport', 'Development', 'HR', 'Finance'];
  tableLoaded = false;
  userParams: any = {};
  pagination: Pagination;
  reverse = true;
  belongToTeam: boolean;
  isLoading = false;


  giveFeedbackForm = new FormGroup({
    sender: new FormControl(),
    reciver: new FormControl(),
    punctuality: new FormControl('', Validators.required),
    id: new FormControl(),
    productivity: new FormControl(),
    commskills: new FormControl(),
    workquality: new FormControl(),
    comments: new FormControl('', [Validators.required]),
  });



  constructor(private userService: UserService,
              private alertify: AlertifyService,
              private modalService: BsModalService,
              private route: ActivatedRoute,
              private dialogService: DialogService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line:no-string-literal
      this.users = data['users'].result;
      // tslint:disable-next-line:no-string-literal
      this.pagination = data['users'].pagination;

      // this.tableLoaded = true;
    });

    if (localStorage.getItem('Manager_Id') !== null) {
      this.loadTeam();
    }

    // this.isLoading = false;

    this.userParams.orderBy = 'asc';
    this.userParams.team = false;
    this.userParams.role = null;
    this.userParams.search = '';
    //  this.pagination.currentPage = 1;
    //  this.pagination.itemsPerPage = 10;

  }


  loadUsers() {
    this.isLoading = true;
    this.userService.getUsers(this.userParams, this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
        this.isLoading = false;
      }, error => {
        this.alertify.error('loadUser');
        this.isLoading = false;
      });
  }


  onClick() {
    if (localStorage.getItem('role') === 'manager') {
      this.userParams.team = true;
      this.userParams.role = 'manager';
      this.pagination.currentPage = 1;
    } else if (localStorage.getItem('role') === 'employee') {
      this.userParams.team = true;
      this.userParams.role = 'employee';
      this.pagination.currentPage = 1;
    } else {
      this.userParams.team = false;
      this.userParams.role = null;
    }
  }

  onEnter(event) {
    this.userParams.search = event;
    this.pagination.currentPage = 1;
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }


  isManager(userFromTable) {
    if (localStorage.getItem('role') === 'manager' && localStorage.getItem('Manager_Id') !== null) {

      this.belongToTeam = false;
      this.userTeam.forEach(user => {
        if (user.id === userFromTable) {
          this.belongToTeam = true;
        }
      });
      return this.belongToTeam;
    } else {
      return false;
    }
  }

  onSortName() {
    if (this.userParams.orderBy === 'asc' || this.userParams.orderBy === 'man-asc' ||
        this.userParams.orderBy === 'man-desc' || this.userParams.orderBy === null) {
      this.userParams.orderBy = 'desc';
      this.reverse = false;
    } else {
      this.userParams.orderBy = 'asc';
      this.reverse = true;
    }
    this.loadUsers();
  }

  onSortManager() {
    if (this.userParams.orderBy === 'asc' || this.userParams.orderBy === 'desc'  ||
        this.userParams.orderBy === null || this.userParams.orderBy === 'man-desc') {
      this.userParams.orderBy = 'man-asc';
      this.reverse = false;
    } else if (this.userParams.orderBy === 'man-asc') {
      this.userParams.orderBy = 'man-desc';
      this.reverse = true;
    }
    this.loadUsers();
  }


  resetFilters() {
    this.userParams.team = false;
    this.userParams.orderBy = null;
    this.filter = '';
    this.userParams.search = '';
    this.loadUsers();
  }

  onDelete(user: any) {
    this.dialogService.openConfirmDialog('Are you sure you want to delete ' + user.name + ' ?')
      .afterClosed().subscribe(res => {
        if (res) {
          this.userService.deleteUser(user.id).subscribe(() => {

            this.alertify.success('User deleted!');
            this.loadUsers();
          }, error => {
            this.alertify.error('Error deleting user!');
          });
        }
      });

  }
  // public openConfirmationDialog() {
  //  this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to ... ?')
  //    .then((confirmed) => console.log('User confirmed:', confirmed))
  // .catch(() => console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)'));
  // }

  // isEmployee() {
  //   return localStorage.getItem('role') !== 'employee';
  // }

  loadTeam() {
    if (localStorage.getItem('role') === 'manager' && localStorage.getItem('Manager_Id') !== null) {
      this.userService.getTeam(localStorage.getItem('id')).subscribe((res: User[]) => {
        this.userTeam = res;
      }, error => {
        this.alertify.error('LoadTeeam');
      });


      //   let params = {
      //   team: true,
      //   role: 'manager',
      //   orderBy: 'asc'
      // };
      //   this.userService.getUsers(params, this.pagination.currentPage, this.pagination.itemsPerPage)
      //                     .subscribe((res: PaginatedResult<User[]>) => {
      //                       this.userTeam = res.result;
      //                       // this.pagination = res.pagination;
      //                     }, error => {
      //                       this.alertify.error('LoadTeeam');
      //                     });
      //   params = null;
    }
  }


}
