import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ProductGalleryComponent } from './product-gallery/product-gallery.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { CheckoutComponent } from './checkout/checkout.component';

export const routes: Routes = [
	{ path: '', component: HomeComponent },
	{
		path: '',
		runGuardsAndResolvers: 'always',
		children: [
			{ path: 'products', component: ProductGalleryComponent },
			{ path: 'products/:id', component: ProductDetailComponent },
			{ path: 'checkout', component: CheckoutComponent }
		]
	}
];
