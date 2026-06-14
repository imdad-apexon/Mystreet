import api from './api';
import type { DashboardOverview } from '../types/dashboard';

export const adminDashboardService = {
  getOverview: async (params?: { trendDays?: number }) => {
    const res = await api.get<DashboardOverview>('/admin/dashboard/overview', { params });
    return res.data;
  }
};