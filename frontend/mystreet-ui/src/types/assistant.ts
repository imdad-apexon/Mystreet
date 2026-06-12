import type { Product } from './product';

export type ChatAssistantRequest = {
  message: string;
  model?: string;
  productLimit?: number;
};

export type ChatAssistantResponse = {
  reply: string;
  recommendedProducts: Product[];
};
