import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-input-date',
  templateUrl: './input-date.component.html',
  styleUrls: ['./input-date.component.css']
})
export class InputDateComponent implements ControlValueAccessor {
@Input() label: string;
@Input() maxDate: Date;
bsConfig: Partial<BsDatepickerConfig>;


constructor(@Self() public ngControl: NgControl) { 
  this.ngControl.valueAccessor = this;
  this.bsConfig = {
    containerClass: 'theme-default',
    dateInputFormat: 'DD MMMM YYYY'
  }
}

  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}

}
