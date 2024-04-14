import { TestBed } from '@angular/core/testing';

import { S_1_1_DirectoryMaintenanceService } from './s-1_1-directory-maintenance.service';

describe('S_1_1_DirectoryMaintenanceService', () => {
  let service: S_1_1_DirectoryMaintenanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(S_1_1_DirectoryMaintenanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
