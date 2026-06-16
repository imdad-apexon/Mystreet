import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    document.title = 'MyStreet - Login';
  }, []);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!email || !password) return setError('All fields are required.');
    try {
      await login(email, password);
      setTimeout(() => navigate('/'), 100);
    } catch {
      setError('Invalid credentials.');
    }
  };

  return (
    <div className="container form-page auth-page">
      <div className="auth-layout">
        <aside className="auth-hero" aria-hidden="true">
          <p className="auth-hero__eyebrow">MyStreet</p>
          <h2>Shop smarter, faster, and with confidence.</h2>
          <p>
            Sign in to save your cart, track orders, and get personalized product recommendations.
          </p>
          <ul className="auth-hero__points">
            <li>Real-time stock updates</li>
            <li>Faster checkout experience</li>
            <li>Order history in one place</li>
          </ul>
        </aside>
        <div className="card form-card auth-card">
          <header className="auth-page__header">
            <h1>Welcome back</h1>
            <p>Login to continue to your MyStreet account.</p>
          </header>
          <form className="auth-form" onSubmit={submit}>
            <div className="form-group">
              <label htmlFor="login-email">Email</label>
              <input
                id="login-email"
                type="email"
                placeholder="you@example.com"
                autoComplete="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="login-password">Password</label>
              <input
                id="login-password"
                placeholder="Enter your password"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={e => setPassword(e.target.value)}
              />
            </div>
            {error && <p className="error">{error}</p>}
            <button className="auth-submit" type="submit">Login</button>
          </form>
          <div className="auth-links">
            <Link to="/products" className="auth-links__primary">Browse products</Link>
            <p className="auth-switch">
              New to MyStreet? <Link to="/register">Create account</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}