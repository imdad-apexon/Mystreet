import api from './api';
import type { Product } from '../types/product';

export const productService = {
  getAll: async (params?: { brand?: string; size?: string; category?: string; minPrice?: number; maxPrice?: number }) => {
    const res = await api.get<Product[]>('/products', { params });
    return res.data;
  },
  searchAi: async (params: { query: string; model?: string; limit?: number }) => {
    const res = await api.get<Product[]>('/products/ai-search', { params });
    return res.data;
  },
  getById: async (id: string) => {
    const res = await api.get<Product>(`/products/${id}`);
    return res.data;
  },
  create: async (payload: Omit<Product, 'id'>) => {
    const res = await api.post<Product>('/products', payload);
    return res.data;
  },
  update: async (id: string, payload: Omit<Product, 'id'>) => {
    const res = await api.put<Product>(`/products/${id}`, payload);
    return res.data;
  },
  remove: async (id: string) => {
    await api.delete(`/products/${id}`);
  }
};