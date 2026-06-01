import axios from 'axios';
import { storage } from './storage';

const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL}/api`
});

api.interceptors.request.use((config) => {
  const token = storage.getToken();

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export default api;