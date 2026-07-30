export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  role: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  role: string;
}

export interface RegisterResponse {
  message: string;
}

export interface CurrentUser {
  username: string;
  role: string;
}
