import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { NZ_MODAL_DATA, NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { KeyValuePair } from '@app/_core/utilities/key-value-pair';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { FunctionService } from '@app/_core/services/system/function.service';
import { FunctionVM } from '@app/_core/models/system/functionvm';
import { NzSpinnerCustomService } from '@app/_core/services/common/nz-spinner.service';
import { NzNotificationCustomService } from '@app/_core/services/nz-notificationCustom.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-form',
  standalone: true,
  imports: [NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzButtonModule, NzSelectModule, NzInputNumberModule],
  templateUrl: './form.component.html',
  styleUrl: './form.component.less'
})
export class FormComponent implements OnInit {

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
  listParentId: KeyValuePair[] | undefined = [];
  itemFunction: FunctionVM = <FunctionVM>{};

  constructor(private fb: FormBuilder, private modal: NzModalRef,
    private functionService: FunctionService,
    private spinService: NzSpinnerCustomService,
    private notification: NzNotificationCustomService,
    private translateService: TranslateService,
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
        this.listParentId = res.data;
      }
    })
  }

  getById(): void {
    this.spinService.show();
    this.functionService.getById(this.id).subscribe({
      next: (res) => {
        if (res.succeeded && res.data)
          this.itemFunction = res.data;
        this.dataForm.patchValue(this.itemFunction);
        this.dataForm.get('id')?.disable();
      }
    }).add(() => this.spinService.hide());
  }

  submitForm(): void {
    if (this.dataForm.valid) {
      console.log(this.dataForm.value);
      // Gửi dữ liệu form
      const formValue = this.dataForm.value as FunctionVM;
      const isAdding = !this.id;
      const apiCall = isAdding ? this.functionService.add(formValue) : this.functionService.edit(this.id, <FunctionVM>{ ...formValue, id: this.id });

      this.spinService.show();

      apiCall.subscribe({
        next: result => {
          const message = isAdding ? 'System.Message.CreateOKMsg' : 'System.Message.UpdateOKMsg';
          const isSuccess = result.succeeded || result;

          this.notification[isSuccess ? 'success' : 'error'](this.translateService.instant(message), this.translateService.instant('System.Caption.' + (isSuccess ? 'Success' : 'Error')));

          if (isSuccess) {
            // Đóng modal và gửi kèm dữ liệu
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
