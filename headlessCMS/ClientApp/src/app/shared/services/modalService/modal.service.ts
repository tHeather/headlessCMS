import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MessageModalComponent } from '../../components/modals/message-modal/message-modal.component';
import { MessageModalData } from '../../components/modals/message-modal/messageModalData';
import { RedirectModalComponent } from '../../components/modals/redirect-modal/redirect-modal.component';
import { RedirectModalData } from '../../components/modals/redirect-modal/redirectModalData';
import { MessageType } from '../../models/messageType';

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  constructor(public dialog: MatDialog) {}

  displayMessageModal(data: MessageModalData): void {
    this.dialog.open(MessageModalComponent, {
      data,
    });
  }

  displayRedirectModal(data: RedirectModalData): void {
    this.dialog.open(RedirectModalComponent, {
      data,
    });
  }

  displayInternalServerErrorMessage(): void {
    this.displayMessageModal({
      messageType: MessageType.error,
      headline: 'Internal server Error',
      text: 'Please try again later.',
    });
  }

  displayNotFoundMessage(): void {
    this.displayMessageModal({
      messageType: MessageType.error,
      headline: 'Not found',
      text: "Can't find requested resource.",
    });
  }

  displayClientSideErrorMessage(): void {
    this.displayMessageModal({
      messageType: MessageType.error,
      headline: 'Unexpected error',
      text: 'Check your internet connection.',
    });
  }

  displayCommonErrorStatusMessage(status: number): void {
    switch (status) {
      case 0:
        this.displayClientSideErrorMessage();
        break;
      case 404:
        this.displayNotFoundMessage();
        break;
      case 500:
        this.displayInternalServerErrorMessage();
        break;
    }
  }
}
