export type AuthResponse = {
  token: string;
  userId: string;
  email: string;
  isAdmin: boolean;
};

export type User = {
  userId: string;
  email: string;
  isAdmin: boolean;
};