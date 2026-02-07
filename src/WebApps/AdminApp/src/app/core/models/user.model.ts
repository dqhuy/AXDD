export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  roles: string[];
  enterpriseId?: string;
  token?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
  expiresAt: string;
}
