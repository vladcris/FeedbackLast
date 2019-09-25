import { FeedbacksService } from './../_services/feedbacks.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { Feedback } from '../_models/feedback.model';

@Injectable()
export class MyFeedbacksResolver implements Resolve<Feedback[]> {
    pageNumber = 1;
    pageSize = 6;
  constructor(private feedbacksService: FeedbacksService,
              private router: Router,
              private alertify: AlertifyService,
              private authService: AuthService) { }

  resolve(route: ActivatedRouteSnapshot): Observable<Feedback[]> {
    // tslint:disable-next-line:no-string-literal
    return this.feedbacksService.getMyFeedbacks(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize).pipe(
      catchError(error => {
        this.alertify.error('Problem retreving data!');
        this.router.navigate(['/all']);
        return of(null);
      })
    );
  }
}
