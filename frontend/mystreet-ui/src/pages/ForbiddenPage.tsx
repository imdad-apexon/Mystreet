import { Link } from 'react-router-dom';

export default function ForbiddenPage() {
  return (
    <div className="container forbidden-wrap">
      <div className="card">
        <h1>403 Forbidden</h1>
        <p>You do not have permission to access this admin page.</p>
        <div className="spacer">
          <Link to="/" className="btn-amazon">Go to home</Link>
        </div>
      </div>
    </div>
  );
}
