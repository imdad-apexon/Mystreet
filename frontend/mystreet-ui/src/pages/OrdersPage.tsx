import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { orderService } from '../services/orderService';
import type { Order } from '../types/order';

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
              <p>Status: {o.status}</p>
              <p>Total: ₹{o.totalAmount}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}