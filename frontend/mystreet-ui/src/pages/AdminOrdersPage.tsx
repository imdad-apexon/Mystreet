import { useEffect, useState } from 'react';
import { orderService } from '../services/orderService';
import { ORDER_STATUS_LABELS, getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';
import { Link } from 'react-router-dom';
import { getImageUrl } from '../utils/urlHelper';
import '../styles/orders.css';

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });

type AdminOrder = {
  id: string;
  status: number;
  totalAmount: number;
  createdAt: string;
  shippingAddress: string;
  paymentMethod: string;
  customerEmail: string | null;
  items?: {
    productId: string;
    productName: string;
    size: string;
    quantity: number;
    unitPrice: number;
    imageUrl: string;
  }[];
};

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<AdminOrder[]>([]);
  const [saving, setSaving] = useState<string | null>(null);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      setOrders(await orderService.all());
    } catch {
      setError('Failed to load orders.');
    }
  };

  useEffect(() => {
    load();
  }, []);

  const onChange = async (id: string, status: number) => {
    setSaving(id);
    setError('');
    try {
      await orderService.updateStatus(id, status);
      setOrders(prev => prev.map(o => (o.id === id ? { ...o, status } : o)));
    } catch {
      setError('Failed to update status.');
    } finally {
      setSaving(null);
    }
  };

  return (
    <div className="orders-page">
      <h1>Admin Orders</h1>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {orders.length === 0 ? (
        <div className="empty">
          <p>No orders.</p>
        </div>
      ) : (
        orders.map(o => (
          <div key={o.id} className="order-card">
            <div className="order-card__header">
              <div className="meta">
                <span className="label">Order placed</span>
                <span className="value">{formatDate(o.createdAt)}</span>
              </div>
              <div className="meta">
                <span className="label">Total</span>
                <span className="value">₹{o.totalAmount}</span>
              </div>
              <div className="meta">
                <span className="label">Customer</span>
                <span className="value">{o.customerEmail ?? '—'}</span>
              </div>
              <div className="order-id">
                Order #
                <strong>{o.id}</strong>
              </div>
            </div>

            <div className="order-card__status-row">
              <span className="title">
                <span className="status-pill" style={getOrderStatusStyle(o.status)}>
                  {getOrderStatusLabel(o.status)}
                </span>
              </span>
              <span style={{ color: '#565959', fontSize: 13 }}>
                Payment: <strong>{o.paymentMethod}</strong>
              </span>
              <label style={{ marginLeft: 16 }}>
                Change status:{' '}
                <select
                  value={o.status}
                  disabled={saving === o.id}
                  onChange={e => onChange(o.id, Number(e.target.value))}
                  style={{ minWidth: 120 }}
                >
                  {Object.entries(ORDER_STATUS_LABELS).map(([val, label]) => (
                    <option key={val} value={val}>{label}</option>
                  ))}
                </select>
              </label>
            </div>

            <div className="order-card__body">
              <div className="order-items">
                {o.items?.map((i, idx) => (
                  <div className="order-item" key={idx}>
                    {i.imageUrl ? (
                      <img
                        className="order-item__image"
                        src={getImageUrl(i.imageUrl)}
                        alt={i.productName}
                        onError={e => ((e.target as HTMLImageElement).style.visibility = 'hidden')}
                      />
                    ) : (
                      <div className="order-item__placeholder">No image</div>
                    )}
                    <div className="order-item__info">
                      <Link to={`/products/${i.productId}`} className="name">
                        {i.productName}
                      </Link>
                      <div className="meta">Size: {i.size}</div>
                      <div className="meta">Qty: {i.quantity}</div>
                    </div>
                    <div className="order-item__price">₹{i.unitPrice}</div>
                  </div>
                ))}
              </div>
              <div className="order-actions">
                <Link to={`/orders/${o.id}`} className="btn-amazon btn-amazon--secondary">View as customer</Link>
              </div>
            </div>
          </div>
        ))
      )}
    </div>
  );
}
