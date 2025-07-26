import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl: string = environment.apiUrl;
  currentUser = signal<User | null>(null);

  constructor() {
    const userString = localStorage.getItem('user');

    if (userString) {
      const user: User = JSON.parse(userString);
      this.currentUser.set({
        username: user.email,
        token: user.token,
        email: user.email
      } as User);
    }
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'auth/login', model).pipe(
      map(user => {
        if (user) {
          this.currentUser.set({
            username: user.email,
            token: user.token,
            email: user.email
          } as User);
        }
        
        return user;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
