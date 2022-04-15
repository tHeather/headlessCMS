import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { Collection } from './collection';
import { ModalService } from '../shared/services/modalService/modal.service';

@Injectable({
  providedIn: 'root',
})
export class CollectionsHttpService {
  constructor(
    private readonly http: HttpClient,
    private readonly modal: ModalService
  ) {}

  createCollection(collection: Collection): Observable<void> {
    return this.http.post<void>(`/collection/create`, collection).pipe(
      catchError((error: HttpErrorResponse) => {
        this.modal.displayCommonErrorStatusMessage(error.status);
        throw error;
      })
    );
  }

  getCollections(): Observable<string[]> {
    return this.http.get<string[]>('/collection/get-all').pipe(
      catchError((error: HttpErrorResponse) => {
        this.modal.displayCommonErrorStatusMessage(error.status);
        throw error;
      })
    );
  }

  getCollectionMetadata(collectionName: string): Observable<Collection> {
    return this.http.get<Collection>(`/collection/${collectionName}`);
  }

  updateCollection(
    collectionName: string,
    collection: Collection
  ): Observable<void> {
    return this.http
      .put<void>(`/collection/${collectionName}`, collection)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.modal.displayCommonErrorStatusMessage(error.status);
          throw error;
        })
      );
  }
}
