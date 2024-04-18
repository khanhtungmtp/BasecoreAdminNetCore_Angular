import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { FormGroup, FormBuilder, ReactiveFormsModule, FormControl, FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { MessageConstants, CaptionConstants } from '@constants/message.enum';
import { UrlRouteConstants } from '@constants/url-route.constants';
import { UserLoginParam } from '@models/auth/auth';
import { AuthService } from '@services/auth/auth.service';
import { InjectBase } from '@utilities/inject-base-app';
import { TranslateModule } from '@ngx-translate/core';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzWaveModule } from 'ng-zorro-antd/core/wave';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzAlertModule } from 'ng-zorro-antd/alert';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NzFormModule, ReactiveFormsModule, NzAlertModule, NzTabsModule, NzGridModule, NzButtonModule, NzInputModule, NzWaveModule, NzCheckboxModule, NzIconModule, RouterLink, NzNotificationModule, TranslateModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent extends InjectBase implements OnInit {
  validateForm: FormGroup<{
    userName: FormControl<string | null>;
    password: FormControl<string | null>;
    remember: FormControl<boolean | null>;
  }> = new FormGroup({
    userName: new FormControl(''),
    password: new FormControl(''),
    remember: new FormControl(true),
  });

  isCapsLockOn: boolean = false;

  user: UserLoginParam = <UserLoginParam>{
  };

  constructor(private fb: FormBuilder, private authService: AuthService) {
    super();
  }

  ngOnInit(): void {
    this.validateForm = this.fb.group({
      userName: [''],
      password: [''],
      remember: [true]
    });
  }

  onKeyUp(event: KeyboardEvent): void {
    if (event.getModifierState && event.getModifierState('CapsLock')) {
      this.isCapsLockOn = true; // Caps Lock is on.
    } else {
      this.isCapsLockOn = false; // Caps Lock is off.
    }
  }

  login(): void {
    if (this.validateForm.valid) {
      this.user.userName = this.validateForm.value.userName as string
      this.user.password = this.validateForm.value.password as string
      this.spinnerService.show();
      this.authService.login(this.user).subscribe({
        next: (res) => {
          console.log('res: ', res);

          this.notification.success(MessageConstants.LOGGED_IN, CaptionConstants.SUCCESS);
          this.spinnerService.hide();
          this.router.navigate([UrlRouteConstants.DASHBOARD]);
        },
        // error: (err) => {
        //   throw err;
        // },
      });

    } else {
      Object.values(this.validateForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }


}
