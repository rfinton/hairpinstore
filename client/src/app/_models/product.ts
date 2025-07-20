export interface Image {
	id: number;
	imageUrl: string;
	altText: string;
	isPrimary: boolean;
}

export interface Product {
	id: number;
	name: string;
	description: string;
	price: number;
	stockQuantity: number;
	sku: string;
	material: string;
	color: string;
	size: string;
	style: string;
	isActive: boolean;
	createdDate: string;
	images: {
		$values: Image[];
	};
}

export interface ItemsWrapper {
	$values: Product[];
}

export interface ProductsDto {
	$id: string;
	items: ItemsWrapper;
	totalCount: number;
	page: number;
	pageSize: number;
	totalPages: number;
}
