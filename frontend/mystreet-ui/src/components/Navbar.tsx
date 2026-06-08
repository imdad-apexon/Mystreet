import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const { totalQty } = useCart();
  const location = useLocation();
  const navigate = useNavigate();

  const isProductsActive =
    location.pathname === '/' ||
    location.pathname === '/products' ||
    location.pathname.startsWith('/products/');

  const navClass = ({ isActive }: { isActive: boolean }) =>
    isActive ? 'nav-link active' : 'nav-link';

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="nav">
      <Link to="/" className="brand">MyStreeT</Link>
      <div className="nav-links">
        {isAuthenticated && (
          <NavLink to="/cart" className={navClass}>
            Cart ({totalQty})
          </NavLink>
        )}
        {isAuthenticated && (
          <NavLink
            to="/products"
            className={isProductsActive ? 'nav-link active' : 'nav-link'}
          >
            Products
          </NavLink>
        )}
        {!isAdmin && isAuthenticated && (
          <NavLink to="/orders" className={navClass}>
            Orders
          </NavLink>
        )}
        {isAdmin && (
          <NavLink to="/admin/products" className={navClass}>
            Admin
          </NavLink>
        )}
        {isAdmin && (
          <NavLink to="/admin/orders" className={navClass}>
            Admin Orders
          </NavLink>
        )}
        {!isAuthenticated ? (
          <>
            <NavLink to="/login" className={navClass}>Login</NavLink>
            <NavLink to="/register" className={navClass}>Register</NavLink>
          </>
        ) : (
          <button onClick={handleLogout}>Logout</button>
        )}
      </div>
    </nav>
  );
}