import { Component, OnInit, ChangeDetectionStrategy, ViewChild, TemplateRef, ChangeDetectorRef, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
import { FormComponent } from './form/form.component';
import { NzNotificationCustomService } from '@app/_core/services/nz-notificationCustom.service';

interface SearchParam {
  ruleName: string;
  desc: string;
}

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
    NzBadgeModule
  ]
})
export class FunctionComponent implements OnInit {
  filter: string = '';
  pagination: Pagination | undefined = <Pagination>{
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
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private functionService = inject(FunctionService);
  private notification = inject(NzNotificationCustomService)

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
      pageSize: this.pagination?.pageSize as number,
      pageNumber: e?.pageIndex || this.pagination?.pageNumber as number
    }
    this.functionService.getFunctionsPaging(this.filter, _pagingParam).pipe(finalize(() => {
      this.tableLoading(false);
    })).subscribe((response => {
      if (response.succeeded) {
        this.dataList = response.data?.result;
        this.pagination = response.data?.pagination;
        this.tableConfig.total = this.pagination?.totalCount as number;
        this.tableConfig.pageIndex = this.pagination?.pageNumber as number;
        this.tableLoading(false);
        this.checkedCashArray = [...this.checkedCashArray];
      }
    }));

  }

  search(): void {
    this.getDataList();
  }

  /*Reset*/
  resetForm(): void {
    this.filter = '';
    this.getDataList();
  }

  /*Expand*/
  toggleCollapse(): void {
    this.isCollapse = !this.isCollapse;
  }

  openModal(id?: string): void {
    const isEditMode = !!id;
    const modal = this.modalSrv.create({
      nzTitle: isEditMode ? 'Function Edit' : 'Function Add',
      nzContent: FormComponent,
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

  del(id: string): void {
    this.modalSrv.confirm({
      nzTitle: 'Are you sure you want to delete it? ',
      nzContent: 'Cannot be recovered after deletion',
      nzOnOk: () => {
        this.tableLoading(true);
        /*The comment is the simulation interface call*/
        this.functionService.deleteFunction(id).subscribe({
          next: (res) => {
            if (res?.succeeded)
              this.notification.success('Success', res.message)
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

  allDel(): void {
    if (this.checkedCashArray.length > 0) {
      this.modalSrv.confirm({
        nzTitle: 'Are you sure you want to delete it? ',
        nzContent: 'Cannot be recovered after deletion',
        nzOnOk: () => {
          const tempArrays: number[] = [];
          this.checkedCashArray.forEach(item => {
            tempArrays.push(item.id);
          });
          this.tableLoading(true);
          // The comment is the call to the simulated interface
          // this.dataService.delFireSys(tempArrays).subscribe(() => {
          //   if (this.dataList.length === 1) {
          //     this.tableConfig.pageIndex--;
          //   }
          //   this.getDataList();
          //   this.checkedCashArray = [];
          // }, error => this.tableLoading(false));
          setTimeout(() => {
            this.message.info(`mảng id (hỗ trợ lưu phân trang):${JSON.stringify(tempArrays)}`);
            this.getDataList();
            this.checkedCashArray = [];
            this.tableLoading(false);
          }, 1000);
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
          title: 'Not displayed by default',
          // width: 130,
          field: 'noShow',
          show: false
        },
        {
          title: 'Id',
          // width: 130,
          field: 'id',
          showSort: true
        },
        {
          title: 'Name',
          // width: 230,
          field: 'name',
          showSort: true
        },
        {
          title: 'Url',
          // width: 200,
          field: 'url',
          tdClassList: ['operate-text']
        },
        {
          title: 'Seq.',
          field: 'sortOrder',
          // width: 80
        },
        {
          title: 'Icon',
          field: 'icon',
          // width: 100
        },
        {
          title: 'Operation',
          tdTemplate: this.operationTpl,
          // width: 120,/
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

  ngOnInit(): void {
    this.initTable();
  }
}
