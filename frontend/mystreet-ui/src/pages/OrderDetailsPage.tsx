import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { orderService } from '../services/orderService';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';
import { getImageUrl } from '../utils/urlHelper';
import '../styles/orders.css';

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });

export default function OrderDetailsPage() {
  const { id } = useParams();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      orderService.getById(id)
        .then(setOrder)
        .finally(() => setLoading(false));
    }
  }, [id]);

  if (loading) return <div className="orders-page"><p>Loading…</p></div>;
  if (!order) return <div className="orders-page"><p>Order not found.</p></div>;

  const itemsSubtotal = (order.items ?? []).reduce(
    (sum: number, i: any) => sum + i.unitPrice * i.quantity,
    0
  );

  return (
    <div className="orders-page">
      <Link to="/orders" className="back-link">← Back to your orders</Link>
      <h1>Order Details</h1>

      <div className="order-card">
        <div className="order-card__header">
          <div className="meta">
            <span className="label">Order placed</span>
            <span className="value">{formatDate(order.createdAt)}</span>
          </div>
          <div className="meta">
            <span className="label">Total</span>
            <span className="value">₹{order.totalAmount}</span>
          </div>
          <div className="meta">
            <span className="label">Payment</span>
            <span className="value">{order.paymentMethod ?? '—'}</span>
          </div>
          <div className="order-id">
            Order #
            <strong>{order.id}</strong>
          </div>
        </div>

        <div className="order-card__status-row">
          <span className="title">
            <span className="status-pill" style={getOrderStatusStyle(order.status)}>
              {getOrderStatusLabel(order.status)}
            </span>
          </span>
        </div>

        <div className="order-card__body">
          <div className="order-items">
            {order.items?.map((i: any, idx: number) => (
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
            <Link to="/" className="btn-amazon">Buy it again</Link>
            <Link to="/orders" className="btn-amazon btn-amazon--secondary">All orders</Link>
          </div>
        </div>

        <div className="detail-summary">
          <div>
            <h3>Shipping address</h3>
            <div className="value">{order.shippingAddress || '—'}</div>
          </div>
          <div>
            <h3>Order summary</h3>
            <div className="summary-table">
              <span className="label">Items subtotal:</span>
              <span className="amount">₹{itemsSubtotal}</span>
              <span className="label">Shipping:</span>
              <span className="amount">Free</span>
              <span className="label grand">Order total:</span>
              <span className="amount grand">₹{order.totalAmount}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}