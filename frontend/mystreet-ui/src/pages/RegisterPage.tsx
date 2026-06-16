import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    document.title = 'MyStreet - Register';
  }, []);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    if (!email || password.length < 6) return setError('Valid email and 6+ char password required.');
    try {
      await register(email, password);
      setSuccess('Registration successful. Redirecting to home...');
      window.setTimeout(() => navigate('/'), 900);
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const status = err.response?.status;
        const responseMessage =
          typeof err.response?.data === 'string'
            ? err.response.data
            : typeof err.response?.data?.message === 'string'
              ? err.response.data.message
              : '';

        if (status === 409 || /already exists?|already registered|duplicate/i.test(responseMessage)) {
          setError('User already exist');
          return;
        }
      }

      setError('Registration Failed');
    }
  };

  return (
    <div className="container form-page auth-page">
      <div className="auth-layout">
        <aside className="auth-hero" aria-hidden="true">
          <p className="auth-hero__eyebrow">Join MyStreet</p>
          <h2>Create your account in under a minute.</h2>
          <p>
            Register once to unlock secure checkout, saved carts, and effortless order tracking.
          </p>
          <ul className="auth-hero__points">
            <li>Personalized shopping journey</li>
            <li>Saved addresses and quicker checkout</li>
            <li>Order updates anytime</li>
          </ul>
        </aside>
        <div className="card form-card auth-card">
          <header className="auth-page__header">
            <h1>Create account</h1>
            <p>Get started and begin browsing products right away.</p>
          </header>
          <form className="auth-form" onSubmit={submit}>
            <div className="form-group">
              <label htmlFor="register-email">Email</label>
              <input
                id="register-email"
                type="email"
                placeholder="you@example.com"
                autoComplete="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="register-password">Password</label>
              <input
                id="register-password"
                placeholder="Use at least 6 characters"
                type="password"
                autoComplete="new-password"
                value={password}
                onChange={e => setPassword(e.target.value)}
              />
            </div>
            {success && <p className="note">{success}</p>}
            {error && <p className="error">{error}</p>}
            <button className="auth-submit" type="submit">Create account</button>
          </form>
          <div className="auth-links">
            <Link to="/products" className="auth-links__primary">Browse products</Link>
            <p className="auth-switch">
              Already have an account? <Link to="/login">Login</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}