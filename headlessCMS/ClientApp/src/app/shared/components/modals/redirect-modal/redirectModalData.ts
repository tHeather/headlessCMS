import { ModalHeaderData } from '../modal-header/modalHeaderData';

export interface RedirectModalData extends ModalHeaderData {
  text: string;
  redirectButtonText: string;
  redirectUrl: string;
}
