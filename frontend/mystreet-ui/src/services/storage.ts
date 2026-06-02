import type { CartItem } from '../types/cart';
import type { User } from '../types/auth';

const TOKEN_KEY = 'mystreet_token';
const USER_KEY = 'mystreet_user';
const CART_KEY_PREFIX = 'mystreet_cart';

const getCartKey = (userId?: string | null) => userId ? `${CART_KEY_PREFIX}_${userId}` : `${CART_KEY_PREFIX}_guest`;

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

  getCart: (userId?: string | null): CartItem[] => {
    const raw = localStorage.getItem(getCartKey(userId));
    return raw ? JSON.parse(raw) : [];
  },
  setCart: (cart: CartItem[], userId?: string | null) => localStorage.setItem(getCartKey(userId), JSON.stringify(cart)),
  clearCart: (userId?: string | null) => localStorage.removeItem(getCartKey(userId))
};