import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RedirectModalComponent } from './redirect-modal.component';

describe('RedirectModalComponent', () => {
  let component: RedirectModalComponent;
  let fixture: ComponentFixture<RedirectModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RedirectModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RedirectModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
