import { DestroyRef, inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { first } from 'rxjs/operators';

import { IsNightKey, ThemeOptionsKey } from '@constants/app.constants';
import { ThemeService } from '@services/common/theme.service';
import { NzSafeAny } from 'ng-zorro-antd/core/types';

import { WindowService } from './window.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

type setThemeProp = 'setIsNightTheme' | 'setThemesMode';
type getThemeProp = 'getIsNightTheme' | 'getThemesMode';

interface InitThemeOption {
  storageKey: string;
  setMethodName: setThemeProp;
  getMethodName: getThemeProp;
}

/*
 * Initialize theme
 * */
@Injectable({
  providedIn: 'root'
})
export class InitThemeService {
  private themesService = inject(ThemeService);
  private windowServe = inject(WindowService);
  destroyRef = inject(DestroyRef);
  themeInitOption: InitThemeOption[] = [
    {
      storageKey: IsNightKey,
      setMethodName: 'setIsNightTheme',
      getMethodName: 'getIsNightTheme'
    },
    {
      storageKey: ThemeOptionsKey,
      setMethodName: 'setThemesMode',
      getMethodName: 'getThemesMode'
    }
  ];

  initTheme(): Promise<void> {
    return new Promise(resolve => {
      this.themeInitOption.forEach(item => {
        const hasCash = this.windowServe.getStorage(item.storageKey);
        if (hasCash) {
          this.themesService[item.setMethodName](JSON.parse(hasCash));
        } else {
          (this.themesService[item.getMethodName]() as Observable<NzSafeAny>).pipe(first(), takeUntilDestroyed(this.destroyRef)).subscribe(res => this.windowServe.setStorage(item.storageKey, JSON.stringify(res)));
        }
      });
      return resolve();
    });
  }
}
