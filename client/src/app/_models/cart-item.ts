// models/cart-item.ts
export interface CartItem {
  productId: number;
  name: string;
  price: number;
  quantity: number;
  image: string;
  totalPrice: number;
}