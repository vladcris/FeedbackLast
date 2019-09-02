import { Component, OnInit, TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
 users: User[];
 id: any;
 modalRef: BsModalRef;
 departamente = ['Nothing', 'Suport', 'Development', 'HR', 'Finance'];

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



 constructor(private http: HttpClient,
             private userService: UserService,
             private alertify: AlertifyService,
             private modalService: BsModalService) { }

 ngOnInit() {
    this.loadUsers();
 }

 onSend() {
  console.log(this.giveFeedbackForm.value);
 }

 openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
 }

 loadUsers() {
  this.userService.getUsers().subscribe((response: User[]) => {
    this.users = response;
  }, error => {
    this.alertify.error(error);
  });


 }

 onDelete(id: any) {
    this.userService.deleteUser(id).subscribe(() => {
      console.log(id);
      this.alertify.success('User deleted!');
    }, error => {
      this.alertify.error('Error deleting user!');
    });
 }


}
