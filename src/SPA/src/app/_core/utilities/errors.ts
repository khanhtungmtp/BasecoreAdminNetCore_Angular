// General error handling
export const throwAthError = (errorMsg: string): never => {
  throw new Error(errorMsg);
};

export const throwModalRefError = (): never => {
  return throwAthError('Maybe your modal component has not introduced override modalRef = inject(NzModalRef);');
};
export const throwModalGetCurrentFnError = (): never => {
  return throwAthError('Maybe your modal component does not introduce the getCurrentValue method, you need to override the implementation');
};
