import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!email || !password) return setError('All fields are required.');
    try {
      await login(email, password);
      navigate('/');
    } catch {
      setError('Invalid credentials.');
    }
  };

  return (
    <div className="container form-page">
      <h1>Login</h1>
      <div className="card form-card">
        <form onSubmit={submit}>
          <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
          <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
          {error && <p className="error">{error}</p>}
          <button type="submit">Login</button>
        </form>
      </div>
      <p><Link to="/register">Create account</Link></p>
    </div>
  );
}