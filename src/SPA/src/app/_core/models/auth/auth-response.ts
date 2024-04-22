export interface AuthResponse {
  id: string;
  username: string;
  email: string | null;
  token: string;
  refreshToken: string;
}
