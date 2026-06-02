import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useCart } from '../context/CartContext';
import { orderService } from '../services/orderService';
import { useAuth } from '../context/AuthContext';

export default function CheckoutPage() {
  const { items, totalAmount, clearCart } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();

  const [shippingAddress, setShippingAddress] = useState('');
  const [paymentMethod, setPaymentMethod] = useState('COD');
  const [error, setError] = useState('');

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!shippingAddress) return setError('Shipping address is required.');
    if (!items.length) return setError('Cart is empty.');

    try {
      const payload = {
        shippingAddress,
        paymentMethod,
        items: items.map(i => ({
          productId: i.productId,
          size: i.size,
          quantity: i.quantity
        }))
      };

      const res = await orderService.create(payload);
      clearCart();
      navigate(`/orders/${res.orderId}`);
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        const message = (err.response?.data as { message?: string } | undefined)?.message;
        setError(message || 'Order placement failed. Please review cart quantities and try again.');
        return;
      }

      setError('Order placement failed. Please try again.');
    }
  };

  return (
    <div className="container form-page">
      <h1>Checkout</h1>
      <p>User: {user?.email}</p>
      <p>Total: ₹{totalAmount.toFixed(2)}</p>
      <form onSubmit={submit}>
        <textarea
          placeholder="Shipping Address"
          value={shippingAddress}
          onChange={e => setShippingAddress(e.target.value)}
        />
        <select value={paymentMethod} onChange={e => setPaymentMethod(e.target.value)}>
          <option value="COD">Cash on Delivery</option>
          <option value="UPI">Mock UPI</option>
        </select>
        {error && <p className="error">{error}</p>}
        <button type="submit">Place Order</button>
      </form>
    </div>
  );
}