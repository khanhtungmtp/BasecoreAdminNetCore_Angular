import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OptionsInterface } from '@app/_core/models/common/types';
import { UserVM } from '@app/_core/models/user-manager/uservm';
import { RoleService } from '@app/_core/services/user-manager/role.service';
import { UserManagerService } from '@app/_core/services/user-manager/user-manager.service';
import { ValidatorsService } from '@app/_core/services/validators/validators.service';
import { fnCheckForm } from '@app/_core/utilities/tools';
import { NzSafeAny } from 'ng-zorro-antd/core/types';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NZ_MODAL_DATA, NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { NzTreeSelectModule } from 'ng-zorro-antd/tree-select';
import { Observable, catchError, of } from 'rxjs';

@Component({
  selector: 'app-user-manager-modal',
  templateUrl: './user-manager-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [FormsModule, NzDatePickerModule, NzFormModule, ReactiveFormsModule, NzGridModule, NzInputModule, NzRadioModule, NzSwitchModule, NzTreeSelectModule, NzSelectModule, NzModalModule]

})
export class UserManagerModalComponent implements OnInit {
  addEditForm!: FormGroup;
  roleOptions: OptionsInterface[] = [];
  isEdit: boolean = false;

  readonly nzModalData: UserVM = inject(NZ_MODAL_DATA);
  private fb = inject(FormBuilder);
  private validatorsService = inject(ValidatorsService);
  private roleService = inject(RoleService);
  private dataService = inject(UserManagerService);
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
    return this.dataService.add(this.addEditForm.value).pipe(
      catchError(error => {
        return of(null);
      }))
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

  initForm(): void {
    this.addEditForm = this.fb.group({
      userName: [null, [Validators.required]],
      fullName: [null, [Validators.required]],
      password: ['@@User123', [Validators.required, this.validatorsService.passwordValidator()]],
      gender: [0],
      isActive: [true],
      dateOfBirth: [null],
      phoneNumber: [null, [this.validatorsService.mobileValidator()]],
      email: [null, [Validators.required, Validators.email]],
      roles: [null, [Validators.required]]
    });
  }

  async ngOnInit(): Promise<void> {
    this.initForm();
    this.isEdit = !!this.nzModalData;
    await Promise.all([this.getRoleList()]);
    if (this.isEdit) {
      this.addEditForm.patchValue(this.nzModalData);
      this.addEditForm.controls['userName'].disable();
      this.addEditForm.controls['password'].disable();
    }
  }
}

