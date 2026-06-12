import AppRouter from './routes/AppRouter';
import Navbar from './components/Navbar';
import ShoppingAssistant from './components/ShoppingAssistant';
import { useAuth } from './context/AuthContext';

export default function App() {
  const { isAuthenticated, isAdmin } = useAuth();

  return (
    <>
      <Navbar />
      <AppRouter />
      {isAuthenticated && !isAdmin && <ShoppingAssistant />}
    </>
  );
}