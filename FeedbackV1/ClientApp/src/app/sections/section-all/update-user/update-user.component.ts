import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from './../../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-update-user',
  templateUrl: './update-user.component.html',
  styleUrls: ['./update-user.component.css']
})
export class UpdateUserComponent implements OnInit {
  user: any = {};
  departments: any;
  managers: any;
  constructor(private userService: UserService,
              private route: ActivatedRoute,
              private router: Router,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadUser();
  }

  loadManagers() {
    this.userService.GetManagers().subscribe((res: User[]) => {
      this.managers = res;
    });
  }


  loadUser() {
    // tslint:disable-next-line:no-string-literal
    this.userService.getUser(this.route.snapshot.params['id']).subscribe(res => {
      this.user = res;
      this.loadDepartments();
      this.loadManagers();
     //  console.log(this.user);
    });
  }

  loadDepartments() {
    this.userService.getDepartments().subscribe(res => {
      this.departments = res;
      // console.log(this.departments);
    });
  }

  onSubmit() {
    // tslint:disable-next-line:no-string-literal
    this.userService.updateRequest(this.route.snapshot.params['id'], this.user).subscribe(() => {
      this.alertify.success('Update succesfull');
    }, error => {
      this.alertify.error('error');
      });
    this.router.navigate(['/all']);
  }

}
