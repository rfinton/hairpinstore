import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ProductService } from '../_services/product.service';
import { ActivatedRoute } from '@angular/router';
import { Product, Image } from '../_models/product';
import { CurrencyPipe } from '@angular/common';
import { AddtocartComponent } from "../addtocart/addtocart.component";

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CurrencyPipe, AddtocartComponent],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent implements OnInit {
  @ViewChild('template', { static: false }) modalTemplate!: TemplateRef<any>;
  private productService = inject(ProductService);
  private route = inject(ActivatedRoute);
  product?: Product;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      console.error('Product ID is not provided');
      return;
    }

    const productId = Number(id);
    this.productService.getProduct(productId).subscribe({
      next: (product) => this.product = product,
      error: err => console.error('Error loading product:', err)
    });
  }

  getImageUrl(): Image {
    if (this.product) {
      const primaryImage = this.product.images.$values.find(image => image.isPrimary);
      return primaryImage ? primaryImage : this.product.images.$values[0];
    }
    return {} as Image;
  }  
}
