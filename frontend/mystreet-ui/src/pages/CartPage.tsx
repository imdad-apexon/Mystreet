import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { getImageUrl } from '../utils/urlHelper';

export default function CartPage() {
  const { items, updateQty, removeItem, totalAmount } = useCart();
  const navigate = useNavigate();

  return (
    <div className="container">
      <h1>Your Cart</h1>
      {items.length === 0 ? (
        <p>Cart is empty.</p>
      ) : (
        <>
          <div className="cart-list">
            {items.map(item => (
              <div key={`${item.productId}-${item.size}`} className="cart-row">
                <img src={getImageUrl(item.imageUrl)} alt={item.name} />
                <div>
                  <h3>{item.name}</h3>
                  <p>{item.brand}</p>
                  <p>Size: {item.size}</p>
                  <p>₹{item.price}</p>
                </div>
                <input
                  type="number"
                  min="1"
                  value={item.quantity}
                  onChange={e => updateQty(item.productId, item.size, Number(e.target.value))}
                />
                <button onClick={() => removeItem(item.productId, item.size)}>Remove</button>
              </div>
            ))}
          </div>
          <h3>Total: ₹{totalAmount}</h3>
          <button onClick={() => navigate('/checkout')}>Checkout</button>
        </>
      )}
      <div className="spacer">
        <Link to="/">Continue Shopping</Link>
      </div>
    </div>
  );
}