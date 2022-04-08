import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrevPageButtonComponent } from './prev-page-button.component';

describe('PrevPageButtonComponent', () => {
  let component: PrevPageButtonComponent;
  let fixture: ComponentFixture<PrevPageButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrevPageButtonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PrevPageButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
