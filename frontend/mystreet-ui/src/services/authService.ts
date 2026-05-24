import api from './api';
import type { AuthResponse } from '../types/auth';

export const authService = {
  register: async (email: string, password: string) => {
    const res = await api.post<AuthResponse>('/auth/register', { email, password });
    return res.data;
  },
  login: async (email: string, password: string) => {
    const res = await api.post<AuthResponse>('/auth/login', { email, password });
    return res.data;
  }
};