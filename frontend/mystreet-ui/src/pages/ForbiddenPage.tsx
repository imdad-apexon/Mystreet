import { Link } from 'react-router-dom';

export default function ForbiddenPage() {
  return (
    <div style={{ maxWidth: 640, margin: '3rem auto', padding: '1rem' }}>
      <h1>403 Forbidden</h1>
      <p>You do not have permission to access this admin page.</p>
      <Link to="/" className="btn-amazon">Go to home</Link>
    </div>
  );
}
