import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { NZ_MODAL_DATA, NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { FunctionService } from '@app/_core/services/system/function.service';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { NzSpinnerCustomService } from '@app/_core/services/common/nz-spinner.service';
import { NzNotificationCustomService } from '@app/_core/services/nz-notificationCustom.service';
import { TranslateService } from '@ngx-translate/core';
import { NzTreeSelectModule } from 'ng-zorro-antd/tree-select';
import { FunctionUtility } from '@app/_core/utilities/function-utility';
import { IconSelComponent } from '@app/admin/shared/biz-components/icon-sel/icon-sel.component';

@Component({
  selector: 'app-function-form',
  standalone: true,
  imports: [IconSelComponent, NzModalModule, ReactiveFormsModule, NzTreeSelectModule, NzFormModule, NzInputModule, NzButtonModule, NzSelectModule, NzInputNumberModule],
  templateUrl: './function-form.component.html',
  styleUrl: './function-form.component.less'
})
export class FunctionFormComponent implements OnInit {
  nodes: any[] = [];
  selIconVisible = false;
  dataForm: FormGroup<{
    id: FormControl<string | null>;
    name: FormControl<string | null>;
    url: FormControl<string | null>;
    sortOrder: FormControl<number | null>;
    parentId: FormControl<string | null>;
    icon: FormControl<string | null>;
  }> = new FormGroup({
    id: new FormControl(''),
    name: new FormControl(''),
    url: new FormControl(''),
    sortOrder: new FormControl(0),
    parentId: new FormControl(''),
    icon: new FormControl('')
  });

  title: string = '';
  titleBtnActionForm: string = '';
  listParentId: FunctionVM[] | undefined = [];
  itemFunction: FunctionVM = <FunctionVM>{};

  constructor(private fb: FormBuilder, private modal: NzModalRef,
    private functionService: FunctionService,
    private spinService: NzSpinnerCustomService,
    private notification: NzNotificationCustomService,
    private translateService: TranslateService,
    private ultility: FunctionUtility,
    private modalRef: NzModalRef,
    @Inject(NZ_MODAL_DATA) public id: string) {
    this.initForm();

  }

  ngOnInit(): void {
    this.getParentIds();
    if (this.id) {
      this.getById();
    }
  }

  seledIcon(e: string): void {
    this.dataForm.get('icon')?.setValue(e);
  }

  initForm() {
    this.dataForm = this.fb.group({
      id: [{ value: '', disabled: false }, Validators.required],
      name: [{ value: '', disabled: false }, Validators.required],
      url: ['', Validators.required],
      sortOrder: [0],
      parentId: ['', Validators.required],
      icon: ['', Validators.required]
    });
  }

  handleCancel(): void {
    this.modal.destroy();
  }

  getParentIds(): void {
    this.functionService.getParentIds().subscribe({
      next: (res) => {
        this.listParentId = this.ultility.UnflatteringForLeftMenu(res);
        const rootParent: any = {
          "id": "ROOT",
          "name": "ROOT",
          "url": "",
          "sortOrder": 0,
          "parentId": "ROOT",
          "icon": "branches",
          "children": []
        }
        this.listParentId.unshift(rootParent);
        this.nodes = this.mapTreeNodes(this.listParentId);
      }
    })
  }

  // map response to tree format ng zorror
  mapTreeNodes(nodes: any[]): any[] {
    return nodes.map(node => ({
      title: node.name,
      key: node.id,
      icon: node.icon,
      children: this.mapTreeNodes(node.children || []),
      isLeaf: node.children.length === 0
    }));
  }

  getById(): void {
    this.spinService.show();
    this.functionService.getById(this.id).subscribe({
      next: (res) => {
        this.itemFunction = res;
        this.dataForm.patchValue(this.itemFunction);
        this.dataForm.get('id')?.disable();
      }
    }).add(() => this.spinService.hide());
  }

  submitForm(): void {
    if (this.dataForm.valid) {
      const formValue = this.dataForm.value as FunctionVM;
      const isAdding = !this.id;
      const apiCall = isAdding ? this.functionService.add(formValue) : this.functionService.edit(this.id, <FunctionVM>{ ...formValue, id: this.id });

      this.spinService.show();

      apiCall.subscribe({
        next: result => {
          const message = isAdding ? 'system.message.createOKMsg' : 'system.message.updateOKMsg';
          const isSuccess = result != '';

          this.notification[isSuccess ? 'success' : 'error'](this.translateService.instant('system.caption.' + (isSuccess ? 'success' : 'error')), this.translateService.instant(message));

          if (isSuccess) {
            this.modalRef.destroy(result);
          }
          this.spinService.hide();
        },
        error: (e) => {
          throw e
        }
      });
    } else {
      // Trigger validation
      Object.values(this.dataForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
