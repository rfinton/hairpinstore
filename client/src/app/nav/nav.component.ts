import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { CartService } from '../_services/cart.service';
import { User } from '../_models/user';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit {
  accountService = inject(AccountService);
  cartService = inject(CartService);

  protected isMobile: boolean = false;
  protected cartItemCount: number = 0;
  protected model: User = {} as User;
  protected breakPointOberser = inject(BreakpointObserver);
  

  ngOnInit(): void {
    if (!this.accountService.currentUser()) return;

    this.breakPointOberser.observe([Breakpoints.XSmall]).subscribe(result => {
      this.isMobile = result.matches;
    });

    this.cartService.cartItems$.subscribe(dto => {
      this.cartItemCount = this.cartService.getTotalItems(dto);
    });
  }

  login() {
    return this.accountService.login(this.model).subscribe({
      next: (user: User) => localStorage.setItem('user', JSON.stringify(user)),
      error: err => console.error('Login failed:', err)
    });
  }

  logout() {
    this.accountService.logout();
    localStorage.removeItem('user');
  }
}
