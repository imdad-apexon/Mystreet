import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const { totalQty } = useCart();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="nav">
      <Link to="/" className="brand">MyStreeT</Link>
      <div className="nav-links">
        {isAuthenticated && <Link to="/cart">Cart ({totalQty})</Link>}
                {isAuthenticated && <Link to="/">Products </Link>}
        {isAuthenticated && <Link to="/orders">Orders</Link>}
        {isAdmin && <Link to="/admin/products">Admin</Link>}
        {!isAuthenticated ? (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        ) : (
          <button onClick={handleLogout}>Logout</button>
        )}
      </div>
    </nav>
  );
}