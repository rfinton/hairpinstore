import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { CartItem } from '../_models/cart-item';
import { HttpClient } from '@angular/common/http';
import { CartDto } from '../_models/cart-dto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  public cartItems: CartItem[] = [];
  public cartItemsSubject = new BehaviorSubject<CartItem[]>(this.cartItems);
  public cartItems$ = this.cartItemsSubject.asObservable();

  constructor() {
    console.log('init cartservice');
    this.getCartItems().subscribe({
      next: (dto) => {
        this.cartItems = dto.items.$values;
        this.cartItemsSubject.next(dto.items.$values);
      }
    });
  }

  getCartItems(): Observable<CartDto> {
    return this.http.get<CartDto>(`${this.baseUrl}/cart`);
  }

  addToCart(item: CartItem): Observable<CartDto> {
    const existingItem = this.cartItems.find(i => i.productId === item.productId);

    if (existingItem) {
      existingItem.quantity += item.quantity;
    } else {
      this.cartItems.push(item);
    }

    let itemDto = {
      ProductId: item.productId,
      Quantity: item.quantity
    };

    return this.http.post<CartDto>(`${this.baseUrl}/cart/add`, itemDto);
  }

  removeFromCart(item: CartItem) {
    const dto = { 
      body: { 
        ProductId: item.productId 
      } 
    };
    return this.http.delete(`${this.baseUrl}/cart/remove`, dto);
  }

  clearCart() {
    // this.cartItems = [];
    // this.cartItemsSubject.next(this.cartItems);
    this.http.delete(`${this.baseUrl}/cart/clear`).subscribe({
      next: _ => this.cartItemsSubject.next([]),
      error: error => console.log(error)
    });
  }

  getTotalItems(dto: CartDto | CartItem[]): number {
    let cart = null;
    if (Array.isArray(dto)) cart = dto;
    else cart = dto.items.$values;
    return cart.reduce((total, item) => total + item.quantity, 0);
  }

  getTotalPrice(dto: CartDto | CartItem[]): number {
    let cart = null;
    if (Array.isArray(dto)) cart = dto;
    else cart = dto.items.$values;
    return cart.reduce((total, item) => total + (item.price * item.quantity), 0);
  }
}
