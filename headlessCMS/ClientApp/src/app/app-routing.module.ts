import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CollectionListComponent } from './collections/collection-list/collection-list.component';
import { CreateCollectionComponent } from './collections/create-collection/create-collection.component';
import { UpdateCollectionComponent } from './collections/update-collection/update-collection.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { WelcomePageComponent } from './welcome-page/welcome-page.component';

const routes: Routes = [
  { path: '', component: WelcomePageComponent },
  {
    path: 'collection',
    children: [
      { path: 'create', component: CreateCollectionComponent },
      { path: 'list', component: CollectionListComponent },
      { path: 'edit/:collectionName', component: UpdateCollectionComponent },
    ],
  },
  { path: '**', component: NotFoundPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
