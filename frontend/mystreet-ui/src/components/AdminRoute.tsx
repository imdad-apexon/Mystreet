import { Navigate } from 'react-router-dom';
import type { ReactElement } from 'react';
import { useAuth } from '../context/AuthContext';
import ForbiddenPage from '../pages/ForbiddenPage';

export default function AdminRoute({ children }: { children: ReactElement }) {
  const { isAuthenticated, isAdmin } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return isAdmin ? children : <ForbiddenPage />;
}