import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AddtocartComponent } from "../addtocart/addtocart.component";
import { Product } from '../_models/product';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, RouterLink, AddtocartComponent],
  templateUrl: './product.component.html',
  styleUrl: './product.component.css'
})
export class ProductComponent {
  @Input() product: Product | null = null;
}
