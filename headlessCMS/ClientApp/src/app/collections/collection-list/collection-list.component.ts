import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CollectionsHttpService } from '../collections-http.service';

@Component({
  selector: 'app-collection-list',
  templateUrl: './collection-list.component.html',
  styleUrls: ['./collection-list.component.scss'],
})
export class CollectionListComponent implements OnInit, OnDestroy {
  collections: string[] = [];
  collectionSubscription!: Subscription | undefined;

  constructor(private collectionHttpService: CollectionsHttpService) {}

  ngOnInit(): void {
    this.collectionSubscription = this.collectionHttpService
      .getCollections()
      .subscribe((data) => {
        this.collections = data;
      });
  }

  ngOnDestroy() {
    this.collectionSubscription?.unsubscribe();
  }
}
