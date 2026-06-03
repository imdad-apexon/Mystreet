import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { productService } from '../services/productService';
import { orderService } from '../services/orderService';
import type { Order } from '../types/order';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';
import { getImageUrl } from '../utils/urlHelper';
import '../styles/orders.css';

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [buyAgainError, setBuyAgainError] = useState('');
  const [buyAgainOrderId, setBuyAgainOrderId] = useState<string | null>(null);
  const { addItem, clearCart } = useCart();
  const navigate = useNavigate();

  const handleBuyAgain = async (order: Order) => {
    setBuyAgainError('');
    const orderItems = order.items ?? [];
    if (!orderItems.length) {
      setBuyAgainError('No items found in this order.');
      return;
    }

    setBuyAgainOrderId(order.id);

    const stockResults = await Promise.all(orderItems.map(async (i) => {
      try {
        const product = await productService.getById(i.productId);
        return { item: i, product };
      } catch {
        return { item: i, product: null };
      }
    }));

    const unavailableNames = stockResults
      .filter(x => !x.product || x.product.stockQty < 1)
      .map(x => x.item.productName);

    const availableItems = stockResults
      .filter(x => x.product && x.product.stockQty > 0)
      .map(x => ({
        productId: x.product!.id,
        name: x.product!.name,
        brand: x.product!.brand,
        imageUrl: x.product!.imageUrl,
        price: x.product!.price,
        size: x.item.size,
        quantity: Math.min(x.item.quantity, x.product!.stockQty)
      }))
      .filter(x => x.quantity > 0);

    if (!availableItems.length) {
      setBuyAgainError(unavailableNames.length ? `${unavailableNames.join(', ')} out of stock.` : 'All items are currently unavailable.');
      setBuyAgainOrderId(null);
      return;
    }

    clearCart();
    for (const i of availableItems) {
      addItem(i);
    }

    if (unavailableNames.length) {
      setBuyAgainError(`${unavailableNames.join(', ')} out of stock.`);
    }

    navigate('/cart');
    setBuyAgainOrderId(null);
  };

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
      {buyAgainError && <p className="error">{buyAgainError}</p>}

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
                Order # <strong>{o.id}</strong>
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
                      <div className="order-item__placeholder">
                        <svg width="32" height="32" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg" style={{ marginRight: 6 }}>
                          <rect x="2" y="2" width="28" height="28" rx="6" fill="#e7e7e7"/>
                          <path d="M8 22l5-6 4 5 3-4 4 5" stroke="#b0b0b0" strokeWidth="2" fill="none"/>
                          <circle cx="11" cy="11" r="2" fill="#b0b0b0"/>
                        </svg>
                        No image
                      </div>
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
                <button
                  type="button"
                  className="btn-amazon btn-amazon--secondary"
                  onClick={() => void handleBuyAgain(o)}
                  disabled={buyAgainOrderId === o.id}
                >
                  Buy it again
                </button>
              </div>
            </div>
          </div>
        ))
      )}
    </div>
  );
}