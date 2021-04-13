import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { APIModule } from './../../api/api.module';

import { ReferenceComponent } from './reference.component';
import { ReferenceDetailsComponent } from './reference-details.component';

const declare = [ReferenceComponent, ReferenceDetailsComponent];

@NgModule({
  declarations: declare,
  imports: [CommonModule, APIModule], 
  exports: declare,
})
export class ReferenceModule { }
