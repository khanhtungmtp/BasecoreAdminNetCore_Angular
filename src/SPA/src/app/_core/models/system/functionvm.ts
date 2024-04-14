// using for menu bar left
export interface FunctionVM {
  id: string;
  name: string;
  url: string;
  sortOrder: number;
  parentId: string;
  icon: string;
  children?: FunctionVM[];
  newLinkFlag: boolean;
  open?: boolean;
  selected?: boolean; // Check or not
  menuType: 'C' | 'F'; // c: menu, f button
}
