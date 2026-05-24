import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const { totalQty } = useCart();

  return (
    <nav className="nav">
      <Link to="/" className="brand">MyStreeT</Link>
      <div className="nav-links">
        <Link to="/cart">Cart ({totalQty})</Link>
        {isAuthenticated && <Link to="/orders">Orders</Link>}
        {isAdmin && <Link to="/admin/products">Admin</Link>}
        {!isAuthenticated ? (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        ) : (
          <button onClick={logout}>Logout</button>
        )}
      </div>
    </nav>
  );
}