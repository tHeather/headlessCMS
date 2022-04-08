import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CreateCollectionComponent } from './collections/create-collection/create-collection.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SidenavComponent } from './sidenav/sidenav.component';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialogModule } from '@angular/material/dialog';
import { MessageModalComponent } from './shared/components/modals/message-modal/message-modal.component';
import { CollectionFormComponent } from './collections/collection-form/collection-form.component';
import { CollectionListComponent } from './collections/collection-list/collection-list.component';
import { MatListModule } from '@angular/material/list';
import { PageHeaderComponent } from './shared/components/page-header/page-header.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { WelcomePageComponent } from './welcome-page/welcome-page.component';
import { UpdateCollectionComponent } from './collections/update-collection/update-collection.component';
import { PrevPageButtonComponent } from './shared/components/prev-page-button/prev-page-button.component';
import { RedirectModalComponent } from './shared/components/modals/redirect-modal/redirect-modal.component';
import { ModalHeaderComponent } from './shared/components/modals/modal-header/modal-header.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    AppComponent,
    CreateCollectionComponent,
    SidenavComponent,
    MessageModalComponent,
    CollectionFormComponent,
    CollectionListComponent,
    PageHeaderComponent,
    NotFoundPageComponent,
    WelcomePageComponent,
    UpdateCollectionComponent,
    PrevPageButtonComponent,
    RedirectModalComponent,
    ModalHeaderComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatSidenavModule,
    MatIconModule,
    MatExpansionModule,
    MatToolbarModule,
    MatDividerModule,
    MatDialogModule,
    MatListModule,
    MatProgressSpinnerModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
