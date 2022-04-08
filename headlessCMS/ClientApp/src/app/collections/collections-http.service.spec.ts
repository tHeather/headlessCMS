import { TestBed } from '@angular/core/testing';

import { CollectionsHttpService } from './collections-http.service';

describe('CollectionsHttpService', () => {
  let service: CollectionsHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CollectionsHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
