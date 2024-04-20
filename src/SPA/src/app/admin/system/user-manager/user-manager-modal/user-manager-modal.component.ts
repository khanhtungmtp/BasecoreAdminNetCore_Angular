import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OptionsInterface } from '@app/_core/models/common/types';
import { User } from '@app/_core/services/auth/account.service';
import { DepartmentService } from '@app/_core/services/system/department.service';
import { RoleService } from '@app/_core/services/user-manager/role.service';
import { ValidatorsService } from '@app/_core/services/validators/validators.service';
import { fnCheckForm } from '@app/_core/utilities/tools';
import { fnAddTreeDataGradeAndLeaf, fnFlatDataHasParentToTree } from '@app/_core/utilities/treeTableTools';
import { NzSafeAny } from 'ng-zorro-antd/core/types';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NZ_MODAL_DATA, NzModalRef } from 'ng-zorro-antd/modal';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { NzTreeSelectModule } from 'ng-zorro-antd/tree-select';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-user-manager-modal',
  templateUrl: './user-manager-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [FormsModule, NzFormModule, ReactiveFormsModule, NzGridModule, NzInputModule, NzRadioModule, NzSwitchModule, NzTreeSelectModule, NzSelectModule]

})
export class UserManagerModalComponent implements OnInit {
  addEditForm!: FormGroup;
  readonly nzModalData: User = inject(NZ_MODAL_DATA);
  roleOptions: OptionsInterface[] = [];
  isEdit = false;
  value?: string;
  deptNodes: NzTreeNodeOptions[] = [];

  private fb = inject(FormBuilder);
  private validatorsService = inject(ValidatorsService);
  private roleService = inject(RoleService);
  private deptService = inject(DepartmentService);

  constructor(private modalRef: NzModalRef) { }

  //This method is if there is asynchronous data that needs to be loaded, add it in this method
  protected getAsyncFnData(modalValue: NzSafeAny): Observable<NzSafeAny> {
    return of(modalValue);
  }

  //Return false to not close the dialog box
  protected getCurrentValue(): Observable<NzSafeAny> {
    if (!fnCheckForm(this.addEditForm)) {
      return of(false);
    }
    return of(this.addEditForm.value);
  }

  getRoleList(): Promise<void> {
    return new Promise<void>(resolve => {
      this.roleService.getRoles().subscribe((res) => {
        this.roleOptions = [];
        res.forEach(({ id, name }) => {
          const obj: OptionsInterface = {
            label: name,
            value: id!
          };
          this.roleOptions.push(obj);
        });
        resolve();
      });
    });
  }

  // getDepartmentList(): Promise<void> {
  //   return new Promise<void>(resolve => {
  //     this.deptService.getDepts({ pageNum: 0, pageSize: 0 }).subscribe(({ list }) => {
  //       list.forEach(item => {
  //         // @ts-ignore
  //         item.title = item.departmentName;
  //         // @ts-ignore
  //         item.key = item.id;
  //       });

  //       const target = fnAddTreeDataGradeAndLeaf(fnFlatDataHasParentToTree(list));
  //       this.deptNodes = target;
  //       resolve();
  //     });
  //   });
  // }

  initForm(): void {
    this.addEditForm = this.fb.group({
      userName: [null, [Validators.required]],
      password: ['a123456', [Validators.required, this.validatorsService.passwordValidator()]],
      gender: [1],
      isActive: [true],
      phoneNumber: [null, [this.validatorsService.mobileValidator()]],
      email: [null, [this.validatorsService.emailValidator()]],
      roleId: [null, [Validators.required]],
      departmentId: [null, [Validators.required]],
      departmentName: [null]
    });
  }

  async ngOnInit(): Promise<void> {
    this.initForm();
    this.isEdit = !!this.nzModalData;
    // await Promise.all([this.getRoleList(), this.getDepartmentList()]);
    await Promise.all([this.getRoleList()]);
    if (this.isEdit) {
      this.addEditForm.patchValue(this.nzModalData);
      this.addEditForm.controls['password'].disable();
    }
  }
}

