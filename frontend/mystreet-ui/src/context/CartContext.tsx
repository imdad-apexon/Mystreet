import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { CartItem } from '../types/cart';
import { storage } from '../services/storage';
import { useAuth } from './AuthContext';

type CartContextType = {
  items: CartItem[];
  addItem: (item: CartItem) => void;
  updateQty: (productId: string, size: string, quantity: number) => void;
  removeItem: (productId: string, size: string) => void;
  clearCart: () => void;
  totalQty: number;
  totalAmount: number;
};

const CartContext = createContext<CartContextType | undefined>(undefined);

export function CartProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const activeUserId = user?.userId ?? null;
  const [items, setItems] = useState<CartItem[]>(() => storage.getCart(activeUserId));

  useEffect(() => {
    setItems(storage.getCart(activeUserId));
  }, [activeUserId]);

  useEffect(() => {
    storage.setCart(items, activeUserId);
  }, [items, activeUserId]);

  const addItem = (item: CartItem) => {
    if (!Number.isFinite(item.quantity) || item.quantity < 1) {
      return;
    }

    setItems(prev => {
      const existing = prev.find(x => x.productId === item.productId && x.size === item.size);
      if (existing) {
        return prev.map(x =>
          x.productId === item.productId && x.size === item.size
            ? { ...x, quantity: x.quantity + item.quantity }
            : x
        );
      }
      return [...prev, item];
    });
  };

  const updateQty = (productId: string, size: string, quantity: number) => {
    const normalizedQuantity = Number.isFinite(quantity) ? Math.floor(quantity) : 0;

    setItems(prev =>
      prev.map(x => x.productId === productId && x.size === size ? { ...x, quantity: normalizedQuantity } : x)
        .filter(x => x.quantity > 0)
    );
  };

  const removeItem = (productId: string, size: string) => {
    setItems(prev => prev.filter(x => !(x.productId === productId && x.size === size)));
  };

  const clearCart = () => setItems([]);

  const totalQty = useMemo(() => items.reduce((sum, x) => sum + x.quantity, 0), [items]);
  const totalAmount = useMemo(() => items.reduce((sum, x) => sum + x.quantity * x.price, 0), [items]);

  return (
    <CartContext.Provider value={{ items, addItem, updateQty, removeItem, clearCart, totalQty, totalAmount }}>
      {children}
    </CartContext.Provider>
  );
}

export const useCart = () => {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used inside CartProvider');
  return ctx;
};