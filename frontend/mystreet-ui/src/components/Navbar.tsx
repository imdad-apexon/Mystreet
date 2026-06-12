import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const { totalQty } = useCart();
  const location = useLocation();
  const navigate = useNavigate();
  const [aiQuery, setAiQuery] = useState('');

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

  const runAiSearch = (rawQuery: string) => {
    const q = rawQuery.trim();
    if (!q) {
      navigate('/products');
      return;
    }

    navigate(`/products?ai=${encodeURIComponent(q)}`);
  };

  useEffect(() => {
    const q = new URLSearchParams(location.search).get('ai') ?? '';
    setAiQuery(q);
  }, [location.search]);

  const handleAiSearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    runAiSearch(aiQuery);
  };

  const handleClearAiSearch = () => {
    setAiQuery('');
    navigate('/products');
  };

  useEffect(() => {
    if (!isAuthenticated || isAdmin) return;

    const currentAi = (new URLSearchParams(location.search).get('ai') ?? '').trim();
    const nextAi = aiQuery.trim();
    if (currentAi === nextAi) return;

    const timeout = setTimeout(() => {
      runAiSearch(aiQuery);
    }, 450);

    return () => clearTimeout(timeout);
  }, [aiQuery, isAdmin, isAuthenticated, location.search]);

  return (
    <nav className="nav">
      <Link to="/" className="brand">MyStreeT</Link>
      {!isAdmin && isAuthenticated && (
        <form className="nav-ai-search" onSubmit={handleAiSearch}>
          <input
            type="text"
            placeholder="AI search: black running shoes under $100"
            value={aiQuery}
            onChange={(e) => setAiQuery(e.target.value)}
            className="nav-ai-search__input"
          />
          {aiQuery.trim().length > 0 && (
            <button type="button" className="nav-ai-search__clear" onClick={handleClearAiSearch}>Clear</button>
          )}
        </form>
      )}
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