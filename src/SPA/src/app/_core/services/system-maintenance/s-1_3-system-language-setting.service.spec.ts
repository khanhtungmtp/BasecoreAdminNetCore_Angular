import { TestBed } from '@angular/core/testing';

import { S_1_3_SystemLanguageSettingService } from './s-1_3-system-language-setting.service';

describe('S_1_3_SystemLanguageSettingService', () => {
  let service: S_1_3_SystemLanguageSettingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(S_1_3_SystemLanguageSettingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
