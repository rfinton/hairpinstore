import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Product, ProductsDto } from '../_models/product';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = environment.apiUrl + 'products';
  private http = inject(HttpClient);

  getProducts(pageNumber?: number, pageSize?: number): Observable<ProductsDto> {
    if (pageNumber != null && pageSize != null) {
      const params = this.setPaginationHeaders(pageNumber, pageSize);
      return this.http.get<ProductsDto>(this.apiUrl, { params });
    }

    return this.http.get<ProductsDto>(this.apiUrl);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(this.apiUrl + '/' + id);
  }

  getProductCount(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/count`);
  }

  setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }
}
