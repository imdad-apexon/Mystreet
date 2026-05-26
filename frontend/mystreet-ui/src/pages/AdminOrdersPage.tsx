import { useEffect, useState } from 'react';
import { orderService } from '../services/orderService';
import { ORDER_STATUS_LABELS, getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';

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
    <div className="container">
      <h1>Admin Orders</h1>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {orders.length === 0 ? (
        <p>No orders.</p>
      ) : (
        <div className="list">
          {orders.map(o => (
            <div key={o.id} className="list-item">
              <p><strong>Order:</strong> {o.id}</p>
              <p><strong>Customer:</strong> {o.customerEmail ?? '—'}</p>
              <p><strong>Date:</strong> {new Date(o.createdAt).toLocaleString()}</p>
              <p><strong>Total:</strong> ₹{o.totalAmount}</p>
              <p><strong>Payment:</strong> {o.paymentMethod}</p>
              <p>
                <strong>Status:</strong>{' '}
                <span style={getOrderStatusStyle(o.status)}>{getOrderStatusLabel(o.status)}</span>
              </p>
              <label>
                Change status:{' '}
                <select
                  value={o.status}
                  disabled={saving === o.id}
                  onChange={e => onChange(o.id, Number(e.target.value))}
                >
                  {Object.entries(ORDER_STATUS_LABELS).map(([val, label]) => (
                    <option key={val} value={val}>{label}</option>
                  ))}
                </select>
              </label>
              {o.items && o.items.length > 0 && (
                <ul style={{ marginTop: 8, paddingLeft: 20 }}>
                  {o.items.map((i, idx) => (
                    <li key={idx}>
                      {i.productName} — Size {i.size} × {i.quantity} @ ₹{i.unitPrice}
                    </li>
                  ))}
                </ul>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
