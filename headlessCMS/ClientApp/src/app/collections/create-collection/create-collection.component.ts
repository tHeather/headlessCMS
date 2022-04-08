import { Component, OnDestroy, ViewChild } from '@angular/core';
import { ModalService } from '../../shared/services/modalService/modal.service';
import { CollectionsHttpService } from '../collections-http.service';
import { MessageType } from 'src/app/shared/models/messageType';
import { CollectionFormComponent } from '../collection-form/collection-form.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-create-collection',
  templateUrl: './create-collection.component.html',
  styleUrls: ['./create-collection.component.scss'],
})
export class CreateCollectionComponent implements OnDestroy {
  createCollectionSubscription: Subscription | undefined;

  @ViewChild(CollectionFormComponent) formComponent!: CollectionFormComponent;

  constructor(
    private collectionHttpService: CollectionsHttpService,
    private modalService: ModalService
  ) {}

  ngOnDestroy() {
    this.createCollectionSubscription?.unsubscribe();
  }

  saveCollection(): void {
    this.formComponent.isLoading = true;
    this.createCollectionSubscription = this.collectionHttpService
      .createCollection(this.formComponent.form.value)
      .subscribe(() => {
        this.formComponent.isLoading = false;
        this.modalService.displayMessageModal({
          messageType: MessageType.success,
          headline: 'Completed successfully',
          text: `Collection ${this.formComponent.name.value} has been successfully saved.`,
        });
        this.formComponent.form.reset();
        this.formComponent.fields.clear();
        this.formComponent.addField();
      });
  }
}
