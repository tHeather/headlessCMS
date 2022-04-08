import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RedirectModalData } from './redirectModalData';

@Component({
  selector: 'app-redirect-modal',
  templateUrl: './redirect-modal.component.html',
  styleUrls: ['./redirect-modal.component.scss'],
})
export class RedirectModalComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: RedirectModalData) {}
}
