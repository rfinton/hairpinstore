import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { loadStripe, Stripe } from '@stripe/stripe-js';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private stripePromise = loadStripe('pk_test_51L6evTKYpPwfE5xPmNTrOZL7WXo15LkR8IHdGQc42UHYmehWuQyXGWn7JjMEU11M7GRWzvoAt52PYR8X50XwVz9800qEIn3x2w');

  createPaymentIntent(amount: number, currency: string = 'usd'): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(`${this.baseUrl}/payments/create-payment-intent`, { amount, currency });
  }

  getStripe(): Promise<Stripe | null> {
    return this.stripePromise;
  }
}
