export type UserRole =
  | "SCM"
  | "Store Officer"
  | "Store Supervisor"
  | "Store Manager"
  | "Loader"
  | "Knitting Supervisor"
  | "Unit Planner"
  | "Quality Officer"
  | "Admin";

export type UserStatus = "Active" | "Inactive";
export type EntityId = string | number;

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthUser {
  id: EntityId;
  name: string;
  username: string;
  email?: string;
  role: UserRole;
  status: UserStatus;
  fullName?: string;
  isActive?: boolean;
  createdAt?: string;
}

export interface LoginResponse {
  token: string;
  user?: AuthUser;
  expiresAt?: string;
}

export interface CreateUserPayload {
  name: string;
  username: string;
  email: string;
  password: string;
  role: UserRole;
}
