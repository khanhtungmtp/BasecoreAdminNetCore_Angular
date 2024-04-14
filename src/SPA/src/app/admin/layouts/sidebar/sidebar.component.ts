import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { MenuItem } from '@models/core/menuItem';
import { TranslateService } from '@ngx-translate/core';
import { PlatformCoreService } from '@services/platform/platform-core.service';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink, NzFormModule, NzMenuModule, NzIconModule, NzLayoutModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

  @Input() isCollapsed: boolean = false;
  @Output() isCollapsedChange = new EventEmitter<boolean>();

  isShow: boolean = false;

  menuResource: MenuItem[] = [];

  constructor(private platformCoreService: PlatformCoreService, private router: Router, private translate: TranslateService,) {
    this.menuResource = this.platformCoreService.getNav();
    this.translate.onLangChange.pipe(takeUntilDestroyed()).subscribe(async () => {
      this.menuResource = this.platformCoreService.getNav();
    });
  }

  // 是否选中菜单
  isSelected(module: string): boolean {
    const u = this.router.url;
    return module === u;
  }

  // 是否展开菜单
  // isOpen(menuItems: MenuItem[]): boolean {
  //   const u = this.router.url;
  //   const foucusMenu = menuItems.find(menuItem => menuItem.module === u);
  //   const isOpen = foucusMenu != null ? true : false;
  //   return isOpen;
  // }
}
