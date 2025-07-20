import { Component, inject, Input, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Product } from '../_models/product';
import { CartService } from '../_services/cart.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-addtocart',
  standalone: true,
  imports: [NgClass],
  templateUrl: './addtocart.component.html',
  styleUrl: './addtocart.component.css'
})
export class AddtocartComponent {
  @Input() product: Product | null = null;
  @Input() align: 'left' | 'center' | 'right' = 'center';
  private accountService = inject(AccountService);
  private cartService = inject(CartService);
  quantity: number = 0;

  incrementQuantity(): void {
    this.quantity++;
  }

  decrementQuantity(): void {
    if (this.quantity > 0) {
      this.quantity--;
    }
  }

  addToCart(product: Product): void {
    if (this.quantity <= 0) {
      alert('Quantity cannot be zero');
      return;
    }

    if (!this.accountService.currentUser()) {
      alert('Please log in to add items to your cart');
      return;
    }

    if (!product) {
      console.error('Product is not defined');
      return;
    }

    let item = {
      productId: product.id,
      name: product.name,
      price: product.price,
      quantity: this.quantity,
      image: product.images.$values.find(image => image.isPrimary)?.imageUrl || '',
      totalPrice: product.price * this.quantity
    };

    this.cartService.addToCart(item).subscribe(dto => {
      this.cartService.cartItemsSubject.next(dto.items.$values);
      this.quantity = 0;
    });
  }
}
