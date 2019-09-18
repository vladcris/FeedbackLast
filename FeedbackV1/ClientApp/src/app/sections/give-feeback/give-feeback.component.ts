import { AuthService } from './../../_services/auth.service';
import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FeedbacksService } from 'src/app/_services/feedbacks.service';
import { ActivatedRoute, Router, Params, RouterLink } from '@angular/router';
import {Feedback} from 'src/app/_models/feedback.model';
import { NgForm } from '@angular/forms';
import { PaginatedResult } from 'src/app/_models/pagination';


@Component({
  selector: 'app-give-feeback',
  templateUrl: './give-feeback.component.html',
  styleUrls: ['./give-feeback.component.css']
})
export class GiveFeebackComponent implements OnInit {

  @ViewChild('requestForm', {static: false}) requestForm: NgForm;
  feedback: any = {};
  feedbacks: Feedback[];
  receiver: any;
  requester: any;
  pending: boolean;
  punct = ['Bad', 'Decent', 'Good', 'Very Good'];
  constructor(private feedbackService: FeedbacksService,
              private route: ActivatedRoute,
              private router: Router,
              private authService: AuthService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      // tslint:disable-next-line:no-string-literal
      this.feedback = data['feedback'];
    });


   }

  // loadFeedback() {
  //   // tslint:disable-next-line:no-string-literal
  //   this.feedbackService.getFeedback(this.route.snapshot.params['feeD_ID'])
  //   .subscribe((feedback: Feedback) => {
  //     this.feedback = feedback;
  //   });

  // }

  submitRequest() {
    this.feedback.pending = false;
    // tslint:disable-next-line:no-string-literal
    this.feedbackService.updateRequest(this.route.snapshot.params['feeD_ID'], this.feedback)
    .subscribe(next => {
      this.requestForm.reset(this.feedback);
    });
    // this.requestForm.reset(this.feedback);
    this.router.navigate(['/myfeedbacks']);
    this.feedbackService.reloadMyFeedbacks.next();

  }

  onSendRequest() {
    this.feedbackService.getMyFeedbacks(this.authService.decodedToken.nameid, 1, 10)
                      .subscribe((res: PaginatedResult<Feedback[]>) => {
                        this.feedbacks = res.result;
                        if (this.feedbacks[1].pending === true ) {
                          this.pending = true;
                          this.feedbackService.requestSend.next(this.pending);
                          // console.log('length:', this.feedbacks.length);
                          // console.log( 'feed de 0:', this.feedbacks[0].pending);

                      } else {
                        this.pending = false;
                        this.feedbackService.requestSend.next(this.pending);
                      }
                      });

  }



}
