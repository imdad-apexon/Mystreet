import api from './api';
import type { Order } from '../types/order';

export const orderService = {
  create: async (payload: any) => {
    const res = await api.post<{ orderId: string }>('/orders', payload);
    return res.data;
  },
  mine: async () => {
    const res = await api.get<Order[]>('/orders/mine');
    return res.data;
  },
  getById: async (id: string) => {
    const res = await api.get(`/orders/${id}`);
    return res.data;
  },
  cancel: async (id: string) => {
    await api.post(`/orders/${id}/cancel`);
  }
};