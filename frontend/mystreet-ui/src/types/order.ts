export type OrderItem = {
  productId: string;
  productName: string;
  size: string;
  quantity: number;
  unitPrice: number;
  imageUrl?: string | null;
};

export type Order = {
  id: string;
  status: number | string;
  totalAmount: number;
  createdAt: string;
  shippingAddress?: string;
  paymentMethod?: string;
  items?: OrderItem[];
};