import { Component, OnInit, ChangeDetectionStrategy, ViewChild, TemplateRef, ChangeDetectorRef, inject, DestroyRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FunctionService } from '@app/_core/services/system/function.service';
import { Pagination, PaginationParam } from '@app/_core/utilities/pagination-utility';
import { AntTableComponent, AntTableConfig, SortFile } from '@app/admin/shared/components/ant-table/ant-table.component';
import { CardTableWrapComponent } from '@app/admin/shared/components/card-table-wrap/card-table-wrap.component';
import { PageHeaderComponent, PageHeaderType } from '@app/admin/shared/components/page-header/page-header.component';
import { WaterMarkComponent } from '@app/admin/shared/components/water-mark/water-mark.component';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzSafeAny } from 'ng-zorro-antd/core/types';
import { NzWaveModule } from 'ng-zorro-antd/core/wave';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { finalize } from 'rxjs';
import { FunctionFormComponent } from './function-form/function-form.component';
import { NzNotificationCustomService } from '@app/_core/services/nz-notificationCustom.service';
import { NzSpinnerCustomService } from '@app/_core/services/common/nz-spinner.service';
import { HasRoleDirective } from '@app/_core/directives/hasrole.directive';
import { ActionCode } from '@app/_core/constants/actionCode';

@Component({
  selector: 'app-function',
  templateUrl: './function.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [
    PageHeaderComponent,
    NzCardModule,
    WaterMarkComponent,
    FormsModule,
    NzFormModule,
    NzGridModule,
    NzInputModule,
    NzButtonModule,
    NzWaveModule,
    NzIconModule,
    CardTableWrapComponent,
    AntTableComponent,
    NzBadgeModule,
    HasRoleDirective
  ]
})
export class FunctionComponent implements OnInit {
  actionCode = ActionCode;
  filter: string = '';
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  }
  @ViewChild('operationTpl', { static: true }) operationTpl!: TemplateRef<NzSafeAny>;
  isCollapse = true;
  tableConfig!: AntTableConfig;
  pageHeaderInfo: Partial<PageHeaderType> = {
    title: 'Function',
    breadcrumb: ['Homepage', 'System', 'Function']
  };
  checkedCashArray: NzSafeAny[] = [];
  dataList: any = [];

  private modalSrv = inject(NzModalService);
  private message = inject(NzMessageService);
  private cdr = inject(ChangeDetectorRef);
  private dataService = inject(FunctionService);
  private notification = inject(NzNotificationCustomService)
  protected spinnerService = inject(NzSpinnerCustomService);
  destroyRef = inject(DestroyRef);
  ngOnInit(): void {
    this.initTable();
  }

  // Triggered when the leftmost checkbox is selected
  selectedChecked(e: any): void {
    this.checkedCashArray = [...e];
  }

  // refresh page
  reloadTable(): void {
    this.message.info('Already refreshed');
    this.getDataList();
  }

  // Trigger table change detection
  tableChangeDectction(): void {
    // Changing the reference triggers change detection.
    this.dataList = [...(this.dataList || [])];
    this.cdr.detectChanges();
  }

  tableLoading(isLoading: boolean): void {
    this.tableConfig.loading = isLoading;
    this.tableChangeDectction();
  }

  getDataList(e?: NzTableQueryParams): void {
    /*-----The actual business request http interface is as follows------*/
    this.tableConfig.loading = true;
    const _pagingParam: PaginationParam = {
      pageSize: e?.pageSize || this.pagination.pageSize,
      pageNumber: this.filter === '' ? (e?.pageIndex || this.pagination.pageNumber) : 1,
    }
    this.dataService.getFunctionsPaging(this.filter, _pagingParam).pipe(finalize(() => {
      this.tableLoading(false);
    })).subscribe((response => {
      this.dataList = response.result;
      this.pagination = response.pagination;
      this.tableConfig.total = this.pagination.totalCount;
      this.tableConfig.pageIndex = this.pagination.pageNumber;
      this.checkedCashArray = [...this.checkedCashArray];
    }));

  }

  search(): void {
    this.notification.success('Success', 'Search success')
    this.getDataList();
  }

  /*Reset*/
  resetForm(): void {
    this.notification.success('Success', 'Reset success')
    this.filter = '';
    this.getDataList();
  }

  openModal(id?: string): void {
    const isEditMode = !!id;
    const modal = this.modalSrv.create({
      nzTitle: isEditMode ? 'Function Edit' : 'Function Add',
      nzContent: FunctionFormComponent,
      nzMaskClosable: false,
      nzData: id
    });

    // Xử lý sự kiện afterClose để nhận dữ liệu từ modal khi nó đóng
    modal.afterClose.subscribe(result => {
      if (result || result?.succeeded) {
        this.getDataList();
      }
    });

  }
  deleteRow(id: string): void {
    this.modalSrv.confirm({
      nzTitle: 'Are you sure you want to delete it? ',
      nzContent: 'Cannot be recovered after deletion',
      nzOnOk: () => {
        this.tableLoading(true);
        /*The comment is the simulation interface call*/
        this.dataService.delete(id).subscribe({
          next: (res) => {
            this.notification.success('Success', `The item '${res}' has been successfully deleted`)
            if (this.dataList.length === 1) {
              this.tableConfig.pageIndex--;
            }
            this.getDataList();
            this.checkedCashArray.splice(this.checkedCashArray.findIndex(item => item.id === id), 1);
          },
          error: () => {
            this.tableLoading(false);
          }
        })

      }
    });
  }

  deleteItemChecked(): void {
    if (this.checkedCashArray.length > 0) {
      this.modalSrv.confirm({
        nzTitle: 'Are you sure you want to delete it? ',
        nzContent: 'Cannot be recovered after deletion',
        nzOnOk: () => {
          const ids: string[] = [];
          this.checkedCashArray.forEach(item => {
            ids.push(item.id);
          });
          this.tableLoading(true);
          this.dataService.deleteRange(ids).subscribe({
            next: (res) => {
              if (res) this.notification.success('Success', "Delete success")
              if (this.dataList.length === 1) {
                this.tableConfig.pageIndex--;
              }
              this.getDataList();
              this.checkedCashArray = [];
            },
            error: () => {
              this.tableLoading(false)
            }
          })
        }
      });
    } else {
      this.message.error('Please check the data');
      return;
    }
  }

  changeSort(e: SortFile): void {
    this.message.info(`Sort field: ${e.fileName}, sorting: ${e.sortDir}`);
  }

  //Modify several items on a page
  changePageSize(e: number): void {
    this.tableConfig.pageSize = e;
  }

  private initTable(): void {
    /*
     * Note that you need to leave one column here without setting the width, so that the list can adapt to the width.
     *
     * */
    this.tableConfig = {
      headers: [
        {
          title: 'Id',
          field: 'id',
          showSort: true
        },
        {
          title: 'Name',
          field: 'name',
          showSort: true
        },
        {
          title: 'Url',
          field: 'url',
          tdClassList: ['operate-text']
        },
        {
          title: 'Seq.',
          field: 'sortOrder'
        },
        {
          title: 'Icon',
          field: 'icon'
        },
        {
          title: 'Operation',
          tdTemplate: this.operationTpl,
          fixed: true,
          fixedDir: 'right'
        }
      ],
      total: 0,
      showCheckbox: true,
      loading: false,
      pageSize: 10,
      pageIndex: 1
    };
  }

}
