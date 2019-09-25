import { AlertifyService } from './../../_services/alertify.service';
import { Pagination, PaginatedResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from './../../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Feedback} from 'src/app/_models/feedback.model';
import { FeedbacksService } from 'src/app/_services/feedbacks.service';


@Component ({
  selector: 'app-section-feedbacks',
  templateUrl: './section-feedbacks.component.html',
  styleUrls: ['./section-feedbacks.component.css'],
  // providers: [FeedbackService]
})
export class SectionFeedbacksComponent implements OnInit {
  myFeedbacks: Feedback[];
  pagination: Pagination;

  constructor(private route: ActivatedRoute,
              private feedbacksService: FeedbacksService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line:no-string-literal
      this.myFeedbacks = data['feedbacks'].result;
      // tslint:disable-next-line:no-string-literal
      this.pagination = data['feedbacks'].pagination;
      this.pagination.currentPage = 1;
      this.pagination.itemsPerPage = 6;
    });

    this.feedbacksService.reloadMyFeedbacks.subscribe( () => {
      this.loadMyFeedbacks();
    });

    this.pagination.currentPage = 1;
    this.pagination.itemsPerPage = 6;
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMyFeedbacks();
  }

  loadMyFeedbacks() {
    this.feedbacksService.getMyFeedbacks(this.authService.decodedToken.nameid, this.pagination.currentPage, this.pagination.itemsPerPage)
                          .subscribe((res: PaginatedResult<Feedback[]>) => {
                            this.myFeedbacks = res.result;
                            this.pagination = res.pagination;
                          }, error => {
                            this.alertify.error(error);
                          });
  }


  onClick5() {
      this.pagination.currentPage = 1;
      this.pagination.itemsPerPage = 5;
  }

  onClick8() {
    this.pagination.currentPage = 1;
    this.pagination.itemsPerPage = 12;
    this.loadMyFeedbacks();
}

resetFilters() {
  this.pagination.currentPage = 1;
  this.pagination.itemsPerPage = 10;
  this.loadMyFeedbacks();
}



}
