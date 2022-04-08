import { Component, Input } from '@angular/core';
import { MessageType } from 'src/app/shared/models/messageType';

@Component({
  selector: 'app-modal-header',
  templateUrl: './modal-header.component.html',
  styleUrls: ['./modal-header.component.scss'],
})
export class ModalHeaderComponent {
  @Input() messageType = MessageType.info;
  @Input() headline = '';
}
