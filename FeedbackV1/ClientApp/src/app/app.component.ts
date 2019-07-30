import { Component, Output} from '@angular/core';
import { ValueComponent } from './value/value.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
@Output() value;


loginBoolean = this.value;

}
