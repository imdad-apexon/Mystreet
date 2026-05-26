export type OrderItem = {
  productId: string;
  productName: string;
  size: string;
  quantity: number;
  unitPrice: number;
};

export type Order = {
  id: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  items?: OrderItem[];
};