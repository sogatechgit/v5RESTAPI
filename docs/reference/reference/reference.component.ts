import { AppMainServiceService } from './../../svc/app-main-service.service';
import { MatDialog } from '@angular/material/dialog';
import { Component, OnInit, Renderer2 } from '@angular/core';
import { ModuleCommon } from '../module.common';

@Component({
  selector: 'app-reference',
  templateUrl: './reference.component.html',
  styleUrls: ['./reference.component.scss']
})
export class ReferenceComponent extends ModuleCommon {
  private dlg: MatDialog;
  constructor(
    public dataSource: AppMainServiceService,
    public dialog: MatDialog,
    public renderer: Renderer2
  ) {
    super(dataSource, dialog, renderer);
  }


  modOnInit(): void {
    this.TableCode = 'rf';

  }

  modAfterViewInit() {}

  RowClick(e: any) {}

  SetupGrid() {}

}
