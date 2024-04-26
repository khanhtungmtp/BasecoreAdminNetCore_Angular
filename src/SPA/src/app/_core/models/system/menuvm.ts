export interface MenuVM {
  id: string;
  name: string;
  url: string;
  sortOrder: number;
  parentId: string;
  icon: string;
  functionId: string;
  commandId: string;
  code: string;
  children?: MenuVM[];
  newLinkFlag: boolean;
  open?: boolean;
  selected?: boolean; // Check or not
  menuType: 'C' | 'F'; // c: menu, f button
}
