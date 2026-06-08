import api from './api';
import type { Order } from '../types/order';

const toArray = <T>(value: unknown): T[] => {
  if (Array.isArray(value)) return value as T[];

  if (value && typeof value === 'object') {
    const wrappedValues = (value as { $values?: unknown }).$values;
    if (Array.isArray(wrappedValues)) return wrappedValues as T[];
  }

  return [];
};

const normalizeOrders = <T extends { items?: unknown; Items?: unknown }>(payload: unknown): T[] => {
  const orders = toArray<T>(payload);

  return orders.map((order) => {
    const normalizedItems = toArray((order as { items?: unknown; Items?: unknown }).items ?? order.Items);
    return {
      ...order,
      items: normalizedItems
    };
  });
};

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
    window.dispatchEvent(new Event('inventory-updated'));
  },
  all: async () => {
    const res = await api.get<unknown>('/orders/all');
    return normalizeOrders<any>(res.data);
  },
  updateStatus: async (id: string, status: number) => {
    await api.put(`/orders/${id}/status`, { status });
    window.dispatchEvent(new Event('inventory-updated'));
  }
};