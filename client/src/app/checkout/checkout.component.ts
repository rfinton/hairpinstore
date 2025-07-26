import { Component, inject, ViewChild, ElementRef, AfterViewInit, OnInit } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CartService } from '../_services/cart.service';
import { CartItem } from '../_models/cart-item';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { StripeService } from '../_services/stripe.service';
import { Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [ReactiveFormsModule, CurrencyPipe, CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit, AfterViewInit {
  @ViewChild('paymentElement', { static: true }) paymentElementRef!: ElementRef;

  protected cartService = inject(CartService);
  private stripeService = inject(StripeService);
  private baseUrl = environment.baseUrl;

  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private payment: StripePaymentElement | null = null;

  protected shippingInfo: FormGroup = new FormGroup({});
  protected totalPrice: number = 0;
  protected totalItems: number = 0;


  constructor() {
    this.shippingInfo = new FormGroup({
      email: new FormControl('', [Validators.email, Validators.required]),
      fullname: new FormControl('', Validators.required),
      address: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      zip: new FormControl('', Validators.required)
    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);

    this.cartService.cartItems$.subscribe(dto => {
      this.totalItems = this.cartService.getTotalItems(dto);
      this.totalPrice = this.cartService.getTotalPrice(dto);
    });
  }

  async ngAfterViewInit() {
    this.stripe = await this.stripeService.getStripe();

    if (!this.stripe) {
      console.error('❌ Stripe failed to load.');
      return;
    }

    this.stripeService.createPaymentIntent(50).subscribe({
      next: resp => {
        this.elements = this.stripe!.elements({
          clientSecret: resp.clientSecret,
          appearance: {
            theme: 'stripe'
          }
        });

        if (!this.elements) return;
        this.payment = this.elements.create('payment');
        this.payment.mount(this.paymentElementRef.nativeElement);
      },
      error: err => console.log(err)
    });
  }

  async onSubmit(event: Event) {
    event.preventDefault();

    if (!this.stripe || !this.payment) {
      alert('Stripe is not properly initialized');
      return;
    }

    // Call elements.submit() to validate and collect data
    const { error: submitError } = await this.elements!.submit();

    if (submitError) {
      return;
    }

    // let total = this.cartService.getTotalPrice(this.cartService.cartItems)
    // Call backend to create payment intent
    this.stripeService.createPaymentIntent(50).subscribe(async res => {
      const { clientSecret } = res;

      const result = await this.stripe!.confirmPayment({
        elements: this.elements!,
        clientSecret,
        confirmParams: {
          return_url: environment.baseUrl
        }
      });

      if (result.error) {
        console.error('❌ Payment failed:', result.error.message);
      }
    });

    if (this.shippingInfo.valid) {
      console.log('Shipping Info:', this.shippingInfo.value);
      // Here you would typically send the shipping info to your backend
      // and handle the response accordingly.
    } else {
      console.error('Form is invalid');
    }
  }

  removeItem(item: CartItem): void {
    console.log(item);
    this.cartService.removeFromCart(item).subscribe({
      next: () => {
        this.cartService.cartItems = this.cartService.cartItems.filter(i => i.productId != item.productId);
        this.cartService.cartItemsSubject.next(this.cartService.cartItems);
      },
      error: err => console.log(err)
    });
  }

  incrementQuantity(item: CartItem): void {
    item.quantity++;
    this.cartService.cartItems.forEach(cartItem => {
      if (cartItem.productId === item.productId) {
        cartItem = item;
        this.totalItems = this.cartService.cartItems.reduce(total => total + item.quantity, 0);
        this.totalPrice = this.cartService.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
        this.cartService.cartItemsSubject.next(this.cartService.cartItems);
      }
    });
  }

  decrementQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      item.quantity--;
      this.cartService.cartItems.forEach(cartItem => {
        if (cartItem.productId === item.productId) {
          cartItem = item;
          this.totalItems = this.cartService.cartItems.reduce(total => total + item.quantity, 0);
          this.totalPrice = this.cartService.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
          this.cartService.cartItemsSubject.next(this.cartService.cartItems);
        }
      });
    }
  }

  trackById(index: number, item: CartItem): number {
    return item.productId;
  }
}
