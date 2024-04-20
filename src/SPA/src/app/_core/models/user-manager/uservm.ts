export interface UserVM {
  id: string;
  userName: string;
  email: string;
  phoneNumber: string;
  fullName: string;
  gender: number; // 0 cho Male (nam), 1 cho Female (nu)
  isActive: boolean;
  lastLoginTime: Date | string | null;
  dateOfBirth: Date | string;
  createdDate: Date | string;
}

