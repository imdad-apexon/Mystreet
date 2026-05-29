import { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { storage } from '../services/storage';
import { authService } from '../services/authService';
import type { User } from '../types/auth';

type AuthContextType = {
  user: User | null;
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  isAdmin: boolean;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(storage.getUser());
  const [token, setToken] = useState<string | null>(storage.getToken());

  useEffect(() => {
    if (user) storage.setUser(user);
    if (token) storage.setToken(token);
  }, [user, token]);

  const login = async (email: string, password: string) => {
    const res = await authService.login(email, password);
    const u: User = { userId: res.userId, email: res.email, isAdmin: res.isAdmin };
    setUser(u);
    setToken(res.token);
    storage.setUser(u);
    storage.setToken(res.token);
  };

  const register = async (email: string, password: string) => {
    await authService.register(email, password);
    // Account created successfully, but don't log in - user must login manually
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    storage.removeUser();
    storage.removeToken();
  };

  return (
    <AuthContext.Provider value={{
      user,
      token,
      login,
      register,
      logout,
      isAuthenticated: !!token,
      isAdmin: !!user?.isAdmin
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
};