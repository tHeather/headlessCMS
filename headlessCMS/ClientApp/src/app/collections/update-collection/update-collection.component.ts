import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { MessageType } from 'src/app/shared/models/messageType';
import { ModalService } from 'src/app/shared/services/modalService/modal.service';
import { Collection } from '../collection';
import { CollectionFormComponent } from '../collection-form/collection-form.component';
import { CollectionsHttpService } from '../collections-http.service';

@Component({
  selector: 'app-update-collection',
  templateUrl: './update-collection.component.html',
  styleUrls: ['./update-collection.component.scss'],
})
export class UpdateCollectionComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  initialCollection!: Collection;
  collectionMetadataSubscription!: Subscription | undefined;
  updateCollectionSubscription: Subscription | undefined;

  @ViewChild(CollectionFormComponent) formComponent!: CollectionFormComponent;

  constructor(
    private collectionHttpService: CollectionsHttpService,
    private modalService: ModalService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const collectionName = this.route.snapshot.paramMap.get('collectionName');

    if (!collectionName) {
      this.displayCollectionNotFoundModal();
      return;
    }

    this.collectionHttpService.getCollectionMetadata(collectionName).subscribe({
      next: (collection) => {
        this.initialCollection = collection;
        this.formComponent.setFormValues(this.initialCollection);
        this.formComponent.isLoading = false;
      },
      error: () => {
        this.displayCollectionNotFoundModal();
      },
    });
  }

  ngAfterViewInit() {
    this.formComponent.isLoading = true;
  }

  ngOnDestroy() {
    this.collectionMetadataSubscription?.unsubscribe();
    this.updateCollectionSubscription?.unsubscribe();
  }

  displayCollectionNotFoundModal() {
    this.modalService.displayRedirectModal({
      messageType: MessageType.error,
      headline: "Can't find collection",
      text: 'Collection not found. Get back to collection list.',
      redirectButtonText: 'Back',
      redirectUrl: 'collection/list',
    });
  }

  updateCollection(): void {
    this.formComponent.isLoading = true;
    this.updateCollectionSubscription = this.collectionHttpService
      .updateCollection(
        this.initialCollection.name,
        this.formComponent.form.value
      )
      .subscribe(() => {
        this.formComponent.isLoading = false;
        this.modalService.displayMessageModal({
          messageType: MessageType.success,
          headline: 'Completed successfully',
          text: `Collection ${this.formComponent.name.value} has been successfully updated.`,
        });
      });
  }
}
