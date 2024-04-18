export interface OperationResult<T = any> {
  statusCode: number;
  succeeded: boolean;
  message: string;
  data?: T; // Optional data property of type T
}

export interface HttpCustomConfig {
  needSuccessInfo?: boolean; // Do you need the "operation successful" prompt?
  showLoading?: boolean; // Whether loading is required
  otherUrl?: boolean; // Is it a third-party interface?
  useApiResponseStructure?: boolean;
}
