import { TestBed } from '@angular/core/testing';

import { S_1_2_programMaintenanceService } from './s-1_2-program-maintenance.service';

describe('S_1_2_programMaintenanceService', () => {
  let service: S_1_2_programMaintenanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(S_1_2_programMaintenanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
