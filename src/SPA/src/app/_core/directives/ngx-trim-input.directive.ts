import {
  Directive,
  ElementRef,
  forwardRef,
  HostListener,
  Renderer2,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

const TRIM_VALUE_ACCESSOR = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => NgxTrimInputDirective),
  multi: true,
};
@Directive({
  selector: '[NgxTrimInput]',
  providers: [TRIM_VALUE_ACCESSOR],
})
export class NgxTrimInputDirective implements ControlValueAccessor {
  _onChange: (value: any) => void = () => { };
  _onTouched: () => any = () => { };

  constructor(private renderer: Renderer2, private elementRef: ElementRef) { }

  writeValue(value: any): void {
    if (value !== undefined && value !== null) {
      const trimmedValue = value.replace(/\s+/g, ' ').trim();
      this.renderer.setProperty(this.elementRef.nativeElement, 'value', trimmedValue);
    }
  }

  registerOnChange(fn: (value: any) => any): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: () => any): void {
    this._onTouched = fn;
  }

  @HostListener('blur', ['$event'])
  _onBlur(event: Event) {
    const element = event.target as HTMLInputElement;
    const val = element.value.replace(/\s+/g, ' ').trim();
    this.writeValue(val);
    this._onChange(val);
  }
}
