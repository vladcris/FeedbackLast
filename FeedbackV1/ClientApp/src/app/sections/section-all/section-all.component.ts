import { Pagination, PaginatedResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormGroup, FormControl, Validators } from '@angular/forms';

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
              private route: ActivatedRoute) { }

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

  onSort() {
    if (this.userParams.orderBy === 'asc') {
      this.userParams.orderBy = 'desc';
      this.reverse = false;
    } else {
      this.userParams.orderBy = 'asc';
      this.reverse = true;
    }
    this.loadUsers();
  }


  resetFilters() {
    this.userParams.team = false;
    this.userParams.orderBy = null;
    this.filter = null;
    this.loadUsers();
  }

  onDelete(id: any) {
    this.userService.deleteUser(id).subscribe(() => {

      this.alertify.success('User deleted!');
    }, error => {
      this.alertify.error('Error deleting user!');
    });
  }

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
