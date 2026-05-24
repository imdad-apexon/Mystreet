import type { CartItem } from '../types/cart';
import type { User } from '../types/auth';

const TOKEN_KEY = 'mystreet_token';
const USER_KEY = 'mystreet_user';
const CART_KEY = 'mystreet_cart';

export const storage = {
  getToken: () => localStorage.getItem(TOKEN_KEY),
  setToken: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  removeToken: () => localStorage.removeItem(TOKEN_KEY),

  getUser: (): User | null => {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  },
  setUser: (user: User) => localStorage.setItem(USER_KEY, JSON.stringify(user)),
  removeUser: () => localStorage.removeItem(USER_KEY),

  getCart: (): CartItem[] => {
    const raw = localStorage.getItem(CART_KEY);
    return raw ? JSON.parse(raw) : [];
  },
  setCart: (cart: CartItem[]) => localStorage.setItem(CART_KEY, JSON.stringify(cart)),
  clearCart: () => localStorage.removeItem(CART_KEY)
};