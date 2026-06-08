import { useState } from 'react';
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
    <div className="container form-page">
      <h1>Register</h1>
      <div className="card form-card">
        <form onSubmit={submit}>
          <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
          <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
          {success && <p className="note">{success}</p>}
          {error && <p className="error">{error}</p>}
          <button type="submit">Register</button>
        </form>
      </div>
      <p><Link to="/products">Browse products</Link></p>
      <p><Link to="/login">Already have an account?</Link></p>
    </div>
  );
}