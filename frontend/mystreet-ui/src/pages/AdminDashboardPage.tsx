import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { adminDashboardService } from '../services/adminDashboardService';
import type { DashboardOverview } from '../types/dashboard';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';
import '../styles/admin-dashboard.css';

const formatCurrency = (value: number) =>
  `₹${value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric'
  });

export default function AdminDashboardPage() {
  const [overview, setOverview] = useState<DashboardOverview | null>(null);
  const [trendDays, setTrendDays] = useState(14);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      setError('');
      try {
        const data = await adminDashboardService.getOverview({ trendDays });
        setOverview(data);
      } catch {
        setError('Failed to load dashboard overview.');
      } finally {
        setLoading(false);
      }
    };

    void load();
  }, [trendDays]);

  const maxTrendRevenue = useMemo(() => {
    if (!overview || overview.salesTrend.length === 0) return 0;
    return Math.max(...overview.salesTrend.map(x => x.revenue));
  }, [overview]);

  const maxTrendOrders = useMemo(() => {
    if (!overview || overview.salesTrend.length === 0) return 0;
    return Math.max(...overview.salesTrend.map(x => x.orders));
  }, [overview]);

  const bestSellerMaxUnits = useMemo(() => {
    if (!overview || overview.bestSellingProducts.length === 0) return 0;
    return Math.max(...overview.bestSellingProducts.map(x => x.unitsSold));
  }, [overview]);

  const maxLowStock = useMemo(() => {
    if (!overview || overview.lowStockProducts.length === 0) return 0;
    return Math.max(...overview.lowStockProducts.map(x => x.stockQty));
  }, [overview]);

  if (loading) {
    return (
      <div className="container admin-dashboard">
        <h1>Admin Dashboard</h1>
        <p>Loading overview...</p>
      </div>
    );
  }

  if (error || !overview) {
    return (
      <div className="container admin-dashboard">
        <h1>Admin Dashboard</h1>
        <p className="error">{error || 'No data available.'}</p>
      </div>
    );
  }

  const { kpis } = overview;

  return (
    <div className="container admin-dashboard">
      <div className="admin-dashboard__header">
        <h1>Admin Dashboard</h1>
        <div className="admin-dashboard__trend-controls">
          <label htmlFor="trendDays">Trend:</label>
          <select
            id="trendDays"
            value={trendDays}
            onChange={e => setTrendDays(Number(e.target.value))}
          >
            <option value={7}>Last 7 days</option>
            <option value={14}>Last 14 days</option>
            <option value={30}>Last 30 days</option>
          </select>
        </div>
      </div>

      <div className="admin-dashboard__kpis">
        <article className="admin-dashboard__kpi-card">
          <span>Total Sales (Units)</span>
          <strong>{kpis.totalSales}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>Total Revenue</span>
          <strong>{formatCurrency(kpis.totalRevenue)}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>Revenue Today</span>
          <strong>{formatCurrency(kpis.revenueToday)}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>Revenue This Week</span>
          <strong>{formatCurrency(kpis.revenueWeek)}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>Revenue This Month</span>
          <strong>{formatCurrency(kpis.revenueMonth)}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>Number of Orders</span>
          <strong>{kpis.numberOfOrders}</strong>
        </article>
        <article className="admin-dashboard__kpi-card">
          <span>New Customers (Month)</span>
          <strong>{kpis.newCustomers}</strong>
        </article>
      </div>

      <section className="admin-dashboard__section">
        <div className="admin-dashboard__section-head">
          <h2>Sales Trend</h2>
        </div>
        <div className="admin-dashboard__trend-chart" role="img" aria-label="Sales trend chart">
          {overview.salesTrend.map(point => {
            const percentage = maxTrendRevenue > 0 ? Math.max((point.revenue / maxTrendRevenue) * 100, 2) : 2;
            return (
              <div key={point.date} className="admin-dashboard__trend-bar-wrap" title={`${formatDate(point.date)} • ${formatCurrency(point.revenue)} • ${point.orders} orders`}>
                <span className="admin-dashboard__trend-value">{formatCurrency(point.revenue)}</span>
                <div className="admin-dashboard__trend-bar" style={{ height: `${percentage}%` }} />
                <span className="admin-dashboard__trend-label">{formatDate(point.date)}</span>
              </div>
            );
          })}
        </div>
      </section>

      <div className="admin-dashboard__split-grid">
        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Orders Trend</h2>
          </div>
          <div className="admin-dashboard__trend-chart admin-dashboard__trend-chart--orders" role="img" aria-label="Orders trend chart">
            {overview.salesTrend.map(point => {
              const percentage = maxTrendOrders > 0 ? Math.max((point.orders / maxTrendOrders) * 100, 4) : 4;
              return (
                <div key={`orders-${point.date}`} className="admin-dashboard__trend-bar-wrap" title={`${formatDate(point.date)} • ${point.orders} orders`}>
                  <span className="admin-dashboard__trend-value">{point.orders}</span>
                  <div className="admin-dashboard__trend-bar admin-dashboard__trend-bar--orders" style={{ height: `${percentage}%` }} />
                  <span className="admin-dashboard__trend-label">{formatDate(point.date)}</span>
                </div>
              );
            })}
          </div>
        </section>

        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Average Order Value</h2>
          </div>
          <div className="admin-dashboard__trend-chart admin-dashboard__trend-chart--aov" role="img" aria-label="Average order value chart">
            {overview.salesTrend.map(point => {
              const averageOrderValue = point.orders > 0 ? point.revenue / point.orders : 0;
              const percentage = maxTrendRevenue > 0 ? Math.max((averageOrderValue / maxTrendRevenue) * 100, 2) : 2;

              return (
                <div key={`aov-${point.date}`} className="admin-dashboard__trend-bar-wrap" title={`${formatDate(point.date)} • AOV ${formatCurrency(averageOrderValue)}`}>
                  <span className="admin-dashboard__trend-value">{formatCurrency(averageOrderValue)}</span>
                  <div className="admin-dashboard__trend-bar admin-dashboard__trend-bar--aov" style={{ height: `${percentage}%` }} />
                  <span className="admin-dashboard__trend-label">{formatDate(point.date)}</span>
                </div>
              );
            })}
          </div>
        </section>
      </div>

      <div className="admin-dashboard__split-grid">
        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Best-Seller Share (Units)</h2>
          </div>
          {overview.bestSellingProducts.length === 0 ? (
            <p className="admin-dashboard__empty">No product share data yet.</p>
          ) : (
            <div className="admin-dashboard__bar-list" role="img" aria-label="Best-seller share chart">
              {overview.bestSellingProducts.map((item) => {
                const width = bestSellerMaxUnits > 0 ? Math.max((item.unitsSold / bestSellerMaxUnits) * 100, 8) : 8;
                return (
                  <div key={`share-${item.productId}`} className="admin-dashboard__bar-row" title={`${item.productName}: ${item.unitsSold} units`}>
                    <span className="admin-dashboard__bar-label">{item.productName}</span>
                    <div className="admin-dashboard__bar-track">
                      <div className="admin-dashboard__bar-fill admin-dashboard__bar-fill--share" style={{ width: `${width}%` }} />
                    </div>
                    <span className="admin-dashboard__bar-value">{item.unitsSold}</span>
                  </div>
                );
              })}
            </div>
          )}
        </section>

        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Low-Stock Risk</h2>
          </div>
          {overview.lowStockProducts.length === 0 ? (
            <p className="admin-dashboard__empty">No low-stock risk detected.</p>
          ) : (
            <div className="admin-dashboard__bar-list" role="img" aria-label="Low-stock risk chart">
              {overview.lowStockProducts.map((item) => {
                const normalized = maxLowStock > 0 ? item.stockQty / maxLowStock : 0;
                const riskWidth = Math.max((1 - normalized) * 100, 8);
                return (
                  <div key={`risk-${item.productId}`} className="admin-dashboard__bar-row" title={`${item.productName}: ${item.stockQty} left`}>
                    <span className="admin-dashboard__bar-label">{item.productName}</span>
                    <div className="admin-dashboard__bar-track">
                      <div className="admin-dashboard__bar-fill admin-dashboard__bar-fill--risk" style={{ width: `${riskWidth}%` }} />
                    </div>
                    <span className="admin-dashboard__bar-value">{item.stockQty}</span>
                  </div>
                );
              })}
            </div>
          )}
        </section>
      </div>

      <div className="admin-dashboard__split-grid">
        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Best-Selling Products</h2>
            <Link to="/admin/products">Manage products</Link>
          </div>
          {overview.bestSellingProducts.length === 0 ? (
            <p className="admin-dashboard__empty">No sales data yet.</p>
          ) : (
            <ul className="admin-dashboard__list">
              {overview.bestSellingProducts.map((item) => (
                <li key={item.productId}>
                  <div>
                    <strong>{item.productName}</strong>
                    <p>{item.unitsSold} units sold</p>
                  </div>
                  <span>{formatCurrency(item.revenue)}</span>
                </li>
              ))}
            </ul>
          )}
        </section>

        <section className="admin-dashboard__section">
          <div className="admin-dashboard__section-head">
            <h2>Low-Stock Products</h2>
            <Link to="/admin/products">View inventory</Link>
          </div>
          {overview.lowStockProducts.length === 0 ? (
            <p className="admin-dashboard__empty">No low-stock products.</p>
          ) : (
            <ul className="admin-dashboard__list">
              {overview.lowStockProducts.map((item) => (
                <li key={item.productId}>
                  <div>
                    <strong>{item.productName}</strong>
                    <p>{item.brand}</p>
                  </div>
                  <span className={item.stockQty <= 3 ? 'admin-dashboard__danger' : ''}>{item.stockQty} left</span>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>

      <section className="admin-dashboard__section">
        <div className="admin-dashboard__section-head">
          <h2>Recent Orders</h2>
          <Link to="/admin/orders">View all orders</Link>
        </div>
        {overview.recentOrders.length === 0 ? (
          <p className="admin-dashboard__empty">No recent orders.</p>
        ) : (
          <div className="admin-dashboard__table-wrap">
            <table className="admin-dashboard__table">
              <thead>
                <tr>
                  <th>Order</th>
                  <th>Customer</th>
                  <th>Items</th>
                  <th>Amount</th>
                  <th>Status</th>
                  <th>Date</th>
                </tr>
              </thead>
              <tbody>
                {overview.recentOrders.map((order) => (
                  <tr key={order.orderId}>
                    <td>
                      <Link to={`/orders/${order.orderId}`}>{order.orderId.slice(0, 8)}...</Link>
                    </td>
                    <td>{order.customerEmail ?? 'Unknown'}</td>
                    <td>{order.itemCount}</td>
                    <td>{formatCurrency(order.totalAmount)}</td>
                    <td>
                      <span style={getOrderStatusStyle(order.status)}>
                        {getOrderStatusLabel(order.status)}
                      </span>
                    </td>
                    <td>{new Date(order.createdAt).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}