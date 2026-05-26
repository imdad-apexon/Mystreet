import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { orderService } from '../services/orderService';
import type { Order } from '../types/order';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';
import { getImageUrl } from '../utils/urlHelper';
import '../styles/orders.css';

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    orderService.mine()
      .then(setOrders)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="orders-page"><p>Loading your orders…</p></div>;
  }

  return (
    <div className="orders-page">
      <h1>Your Orders</h1>

      {orders.length === 0 ? (
        <div className="empty">
          <p>You haven't placed any orders yet.</p>
          <Link to="/" className="btn-amazon" style={{ marginTop: 12 }}>Shop now</Link>
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
                <span className="label">Ship to</span>
                <span className="value" title={o.shippingAddress}>
                  {o.shippingAddress
                    ? (o.shippingAddress.length > 30
                        ? o.shippingAddress.slice(0, 30) + '…'
                        : o.shippingAddress)
                    : '—'}
                </span>
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
              {o.paymentMethod && (
                <span style={{ color: '#565959', fontSize: 13 }}>
                  Payment: <strong>{o.paymentMethod}</strong>
                </span>
              )}
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
                <Link to={`/orders/${o.id}`} className="btn-amazon">View order details</Link>
                <Link to="/" className="btn-amazon btn-amazon--secondary">Buy it again</Link>
              </div>
            </div>
          </div>
        ))
      )}
    </div>
  );
}