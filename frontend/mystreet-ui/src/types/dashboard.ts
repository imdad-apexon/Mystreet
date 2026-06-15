export type DashboardKpis = {
  totalSales: number;
  totalRevenue: number;
  revenueToday: number;
  revenueWeek: number;
  revenueMonth: number;
  numberOfOrders: number;
  newCustomers: number;
};

export type BestSellingProduct = {
  productId: string;
  productName: string;
  unitsSold: number;
  revenue: number;
};

export type LowStockProduct = {
  productId: string;
  productName: string;
  brand: string;
  stockQty: number;
};

export type RecentOrder = {
  orderId: string;
  customerEmail: string | null;
  totalAmount: number;
  status: number;
  itemCount: number;
  createdAt: string;
};

export type SalesTrendPoint = {
  date: string;
  revenue: number;
  orders: number;
};

export type DashboardOverview = {
  kpis: DashboardKpis;
  bestSellingProducts: BestSellingProduct[];
  lowStockProducts: LowStockProduct[];
  recentOrders: RecentOrder[];
  salesTrend: SalesTrendPoint[];
};