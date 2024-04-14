import { ComponentFixture, TestBed } from '@angular/core/testing';

import { S11DirectoryMaintenanceComponent } from './s-1-1-directory-maintenance.component';

describe('S11DirectoryMaintenanceComponent', () => {
  let component: S11DirectoryMaintenanceComponent;
  let fixture: ComponentFixture<S11DirectoryMaintenanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [S11DirectoryMaintenanceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(S11DirectoryMaintenanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
