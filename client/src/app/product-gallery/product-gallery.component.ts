import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../_services/product.service';
import { Product } from '../_models/product';
import { ProductComponent } from "../product/product.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-gallery',
  standalone: true,
  imports: [ProductComponent, CommonModule],
  templateUrl: './product-gallery.component.html',
  styleUrl: './product-gallery.component.css'
})
export class ProductGalleryComponent implements OnInit {
  private productService = inject(ProductService);
  products: Product[] = [];
  pageSize: number = 4;
  pageNumber: number = 1;
  totalPages: number[] = [];

  ngOnInit(): void {
    this.productService.getProductCount().subscribe({
      next: count => {
        let pages = Math.ceil(count / this.pageSize);
        this.totalPages = Array.from({ length: pages }, (_, i) => i + 1);
      },
      error: err => console.log(err)
    });

    this.loadProducts();
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages.length) return;
    this.pageNumber = page;
    this.loadProducts();
  }

  private loadProducts() {
    this.productService.getProducts(this.pageNumber, this.pageSize).subscribe({
      next: res => this.products = res.items.$values.slice(0, this.pageSize),
      error: err => console.log(err)
    });
  }
}
