import { Component } from '@angular/core';
import { ProductGalleryComponent } from "../product-gallery/product-gallery.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ProductGalleryComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
