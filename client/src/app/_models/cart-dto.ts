import { CartItem } from "./cart-item";

export interface CartDto {
	items: {
		$values: CartItem[]
	};
}