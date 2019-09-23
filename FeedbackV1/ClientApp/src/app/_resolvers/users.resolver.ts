import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import {catchError} from 'rxjs/operators';


@Injectable()
export class UsersResolver implements Resolve<User[]> {

userParams: any = {
    team: false,
    orderBy: 'asc',
    search: '',
    role: null
};

    pageNumber = 1;
    pageSize = 10;

    constructor(private userService: UserService,
                private router: Router,
                private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        // tslint:disable-next-line:no-string-literal
        return this.userService.getUsers(this.userParams, this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem retreving data!');
                this.router.navigate(['/all']);
                return of(null);
            })
        );
    }
}
