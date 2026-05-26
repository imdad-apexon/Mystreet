import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { orderService } from '../services/orderService';
import { getOrderStatusLabel, getOrderStatusStyle } from '../utils/orderStatus';

export default function OrderDetailsPage() {
  const { id } = useParams();
  const [order, setOrder] = useState<any>(null);

  useEffect(() => {
    if (id) orderService.getById(id).then(setOrder);
  }, [id]);

  if (!order) return <div className="container">Loading...</div>;

  return (
    <div className="container">
      <h1>Order Details</h1>
      <p>Order ID: {order.id}</p>
      <p>Status: <span style={getOrderStatusStyle(order.status)}>{getOrderStatusLabel(order.status)}</span></p>
      <p>Total: ₹{order.totalAmount}</p>
      <p>Address: {order.shippingAddress}</p>
      <div className="list">
        {order.items?.map((i: any, idx: number) => (
          <div key={idx} className="list-item">
            <p>{i.productName}</p>
            <p>Size: {i.size}</p>
            <p>Qty: {i.quantity}</p>
            <p>₹{i.unitPrice}</p>
          </div>
        ))}
      </div>
    </div>
  );
}