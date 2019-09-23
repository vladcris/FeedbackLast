import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from './../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { PaginatedResult } from 'src/app/_models/pagination';


@Component({
  selector: 'app-admin-section',
  templateUrl: './admin-section.component.html',
  styleUrls: ['./admin-section.component.css']
})
export class AdminSectionComponent implements OnInit {
  filter: string;
  users: User[];
  usersTeam: User[];
  userParams: any = {
  };
  data: any;
  searchKey: string;
  departamente = ['Nothing', 'Suport', 'Development', 'HR', 'Finance'];
  uncheckableRadioModel = 'Left';

    // sorting
    itemsPerPage = 10;
    p = 1;
    key = 'name'; // set default
    reverse = false;
    sort(key) {
      this.key = key;
      this.reverse = !this.reverse;
    }



  constructor(private userService: UserService,
              private route: ActivatedRoute,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line:no-string-literal
      this.users = data['users'].result;
    });

    this.userParams.userId = this.authService.decodedToken.nameid;
    this.loadUsers();

  }

    loadUsers() {
      this.userService.getAllUsers(this.userParams).subscribe((data: PaginatedResult<User[]>) => {
        this.usersTeam = data.result;
    });
  }

    onchange(user: any) {
      user.isDeleted = !user.isDeleted;
      this.userService.adminSoftDelete(user).subscribe(() => {
        this.alertify.success('Succesfull Update!');
        // this.loadUsers();
      },
        error => {
          this.alertify.error('Something went wrong!');
      });
      console.log(user);
    }

    onDelete() {
      alert('Are you serious!?');
    }





}


