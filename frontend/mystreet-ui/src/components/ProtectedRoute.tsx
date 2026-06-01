import { Navigate } from 'react-router-dom';
import type { ReactElement } from 'react';
import { useAuth } from '../context/AuthContext';

export default function ProtectedRoute({ children }: { children: ReactElement }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? children : <Navigate to="/login" replace />;
}