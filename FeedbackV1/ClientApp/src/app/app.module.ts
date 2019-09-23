import { AdminSectionComponent } from './sections/admin-section/admin-section.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import {EmployeesService} from './/_services/employees.service';
import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { BsDropdownModule, ModalModule, ProgressbarModule, RatingModule, PaginationModule } from 'ngx-bootstrap';
import { UserService } from './_services/user.service';
import { JwtModule } from '@auth0/angular-jwt';
import { Ng2SearchPipeModule } from 'ng2-search-filter'; // importing the module
import { Ng2OrderModule } from 'ng2-order-pipe'; // importing the module
import { NgxPaginationModule } from 'ngx-pagination'; // <-- import the module
import { TooltipModule } from 'ngx-bootstrap/tooltip';


import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SectionAllComponent } from './sections/section-all/section-all.component';
import { SectionFeedbacksComponent } from './sections/section-feedbacks/section-feedbacks.component';
import { SectionAdduserComponent } from './sections/section-adduser/section-adduser.component';
import { ViewfeedbackComponent } from './sections/viewfeedback/viewfeedback.component';
import { FeedbackComponent } from './sections/section-feedbacks/feedback/feedback.component';
import { AppRoutingModule } from './app-routing.module';
import { FeedbackListComponent } from './sections/viewfeedback/feedback-list/feedback-list.component';
import { FeedbackDetailComponent } from './sections/viewfeedback/feedback-detail/feedback-detail.component';
import { FeedbackItemComponent } from './sections/viewfeedback/feedback-list/feedback-item/feedback-item.component';
import { ViewstartComponent } from './sections/viewfeedback/viewstart/viewstart.component';
import { GiveFeebackComponent } from './sections/give-feeback/give-feeback.component';
import { LoginComponent } from './login/login.component';
import {RequestComponent} from './sections/request/request.component';
import { ImportComponent } from './sections/import/import.component';
import { MyfeedbackDetailComponent } from './sections/section-feedbacks/myfeedback-detail/myfeedback-detail.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { GiveComponent } from './sections/section-all/give/give.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { UpdateUserComponent } from './sections/section-all/update-user/update-user.component';
import { UsersTeamResolver } from './_resolvers/users-team.resolver';
import { MyFeedbacksResolver } from './_resolvers/myFeedbacks.resolver';
import { UsersResolver } from './_resolvers/users.resolver';
import { GiveFeedbackResolver } from './_resolvers/give-feedback.resolver';
import { ReceiverFeedbacksResolver } from './_resolvers/receiver-feedbacks.resolver';
import { RequestResolver } from './_resolvers/request.resolver';
import {TimeAgoPipe} from 'time-ago-pipe';
import { LoadingSpinnerComponent } from './shared/loading-spinner';
import { MaterialModule } from './material/material.module';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MatDialogModule, MatDialogClose } from '@angular/material/dialog';
import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';
import { TabsModule } from 'ngx-bootstrap/tabs';










export function tokenGetter() {
   return localStorage.getItem('token');
}



@NgModule({
   declarations: [
      AppComponent,
      NavbarComponent,
      SidebarComponent,
      SectionAllComponent,
      SectionFeedbacksComponent,
      SectionAdduserComponent,
      ViewfeedbackComponent,
      FeedbackComponent,
      FeedbackListComponent,
      FeedbackDetailComponent,
      FeedbackItemComponent,
      ViewstartComponent,
      GiveFeebackComponent,
      FeedbackItemComponent,
      LoginComponent,
      RequestComponent,
      ImportComponent,
      MyfeedbackDetailComponent,
      HomeComponent,
      GiveComponent,
      UpdateUserComponent,
      TimeAgoPipe,
      LoadingSpinnerComponent,
      AdminSectionComponent,
      ConfirmDialogComponent,
   ],
   imports: [
      BrowserModule,
       AppRoutingModule,
       MatDialogModule,
       MaterialModule,
       Ng2SearchPipeModule, // including into imports
       Ng2OrderModule, // importing the sorting package here
       NgxPaginationModule,
      ReactiveFormsModule,
      FormsModule,
      HttpClientModule,
      TooltipModule.forRoot(),
      ButtonsModule.forRoot(),
      PaginationModule.forRoot(),
      TabsModule.forRoot(),
      BsDropdownModule.forRoot(),
      ModalModule.forRoot(),
      ProgressbarModule.forRoot(),
      RatingModule.forRoot(),
      JwtModule.forRoot({
         config: {
            // tslint:disable-next-line:object-literal-shorthand
            tokenGetter: tokenGetter,
          whitelistedDomains: ['localhost:5000'],
          blacklistedRoutes: ['localhost:5000/api/auth']

         }
      })

   ],
   providers: [
      EmployeesService,
      AuthService,
      AlertifyService,
      AuthGuard,
      UserService,
     UserDetailResolver,
     UsersTeamResolver,
     MyFeedbacksResolver,
     UsersResolver,
     GiveFeedbackResolver,
     ReceiverFeedbacksResolver,
     RequestResolver
   ],
   bootstrap: [
      AppComponent
   ],
   entryComponents: [ConfirmDialogComponent]
})
export class AppModule { }
