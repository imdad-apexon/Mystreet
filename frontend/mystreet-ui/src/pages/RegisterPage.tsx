import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!email || password.length < 6) return setError('Valid email and 6+ char password required.');
    try {
      await register(email, password);
      navigate('/login');
    } catch {
      setError('Registration failed.');
    }
  };

  return (
    <div className="container form-page">
      <h1>Register</h1>
      <div className="card form-card">
        <form onSubmit={submit}>
          <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
          <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
          {error && <p className="error">{error}</p>}
          <button type="submit">Register</button>
        </form>
      </div>
      <p><Link to="/login">Already have an account?</Link></p>
    </div>
  );
}