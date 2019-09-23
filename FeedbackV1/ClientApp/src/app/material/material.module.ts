import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as Material from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDialogClose } from '@angular/material/dialog'







@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    Material.MatTableModule,
    Material.MatIconModule,
    Material.MatPaginatorModule,
    Material.MatSortModule,
    BrowserAnimationsModule,
    Material.MatFormFieldModule,
    Material.MatDialogModule,
    // Material.MatDialogClose


  ],
  exports : [
    Material.MatTableModule,
    Material.MatIconModule,
    Material.MatPaginatorModule,
    Material.MatSortModule,
    BrowserAnimationsModule,
    Material.MatFormFieldModule,
    // Material.MatDialogClose

  ]

})
export class MaterialModule { }