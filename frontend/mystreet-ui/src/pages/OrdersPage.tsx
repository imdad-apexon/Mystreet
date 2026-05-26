import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { orderService } from '../services/orderService';
import type { Order } from '../types/order';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);

  useEffect(() => {
    orderService.mine().then(setOrders);
  }, []);

  return (
    <div className="container">
      <h1>My Orders</h1>
      {orders.length === 0 ? (
        <p>No orders yet.</p>
      ) : (
        <div className="list">
          {orders.map(o => (
            <div key={o.id} className="list-item">
              <Link to={`/orders/${o.id}`}>Order {o.id}</Link>
              <p>Date: {new Date(o.createdAt).toLocaleString()}</p>
              <p>Status: <span style={getOrderStatusStyle(o.status)}>{getOrderStatusLabel(o.status)}</span></p>
              <p>Total: ₹{o.totalAmount}</p>
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