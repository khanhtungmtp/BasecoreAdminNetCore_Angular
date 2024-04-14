export interface OperationResult<T = any> {
  statusCode: number;
  succeeded: boolean;
  message: string;
  data?: T; // Optional data property of type T
}
