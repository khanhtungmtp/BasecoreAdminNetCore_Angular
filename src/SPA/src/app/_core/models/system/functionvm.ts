// using for menu bar left
export interface FunctionVM {
  id: string;
  name: string;
  url: string;
  sortOrder: number;
  parentId: string;
  icon: string;
  children?: FunctionVM[];
}
