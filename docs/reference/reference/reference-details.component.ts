import { Component, KeyValueDiffers } from '@angular/core';
import { AppDataset } from './../../svc/app-dataset.service';
import { DetailsCommon } from './../../cmp/details.common';

@Component({
  selector: 'app-reference-details',
  templateUrl: './reference-details.component.html',
  styleUrls: ['./reference-details.component.scss']
})
export class ReferenceDetailsComponent   extends DetailsCommon {
  constructor(public differs: KeyValueDiffers) {
    super(differs);

    // this section will allow customized parameter settings for the details popup
    this.popHeight = 315;
    this.tabHeight = 247;
    // this.popButtons= []
    this.titleEdit = 'Edit Reference';
    this.titleNew = 'New Reference File';

    // set view configurations
  }

  ngOnInit(): void {
  }

}
