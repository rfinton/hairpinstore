using HairpinStore.Models;
using Microsoft.EntityFrameworkCore;

namespace HairpinStore.Data;

public static class HairpinDataSeeder
{
	public static async Task SeedHairpinDataAsync(ApplicationDbContext context)
	{
		// Check if products already exist
		if (await context.Products.AnyAsync())
		{
			return; // Database has been seeded
		}

		var products = new List<Product>
				{
                // Bobby Pins
                new Product
					 {
						  Name = "Classic Black Bobby Pins - 50 Pack",
						  Description = "Essential black bobby pins for everyday styling. Smooth finish prevents hair snagging. Perfect for securing loose strands and creating sleek updos.",
						  Price = 8.99m,
						  StockQuantity = 150,
						  SKU = "BP-BLACK-50",
						  Material = "Metal",
						  Color = "Black",
						  Size = "Small",
						  Style = "Bobby Pin",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Gold Bobby Pins - 25 Pack",
						  Description = "Elegant gold-toned bobby pins that blend seamlessly with blonde and light brown hair. Rust-resistant coating ensures long-lasting use.",
						  Price = 12.49m,
						  StockQuantity = 75,
						  SKU = "BP-GOLD-25",
						  Material = "Metal",
						  Color = "Gold",
						  Size = "Small",
						  Style = "Bobby Pin",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Rose Gold Bobby Pins - 30 Pack",
						  Description = "Trendy rose gold bobby pins perfect for modern hairstyles. Non-slip grip keeps hair securely in place all day long.",
						  Price = 14.99m,
						  StockQuantity = 100,
						  SKU = "BP-ROSEGOLD-30",
						  Material = "Metal",
						  Color = "Rose Gold",
						  Size = "Small",
						  Style = "Bobby Pin",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },

                // Hair Clips
                new Product
					 {
						  Name = "Tortoiseshell Hair Clip",
						  Description = "Stylish tortoiseshell pattern hair clip perfect for medium to thick hair. Secure spring mechanism holds hair firmly without causing damage.",
						  Price = 16.99m,
						  StockQuantity = 45,
						  SKU = "HC-TORTOISE-M",
						  Material = "Plastic",
						  Color = "Tortoiseshell",
						  Size = "Medium",
						  Style = "Hair Clip",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Minimalist Black Hair Clip",
						  Description = "Sleek black hair clip with a modern minimalist design. Perfect for professional settings and everyday wear.",
						  Price = 18.99m,
						  StockQuantity = 60,
						  SKU = "HC-BLACK-MIN",
						  Material = "Plastic",
						  Color = "Black",
						  Size = "Medium",
						  Style = "Hair Clip",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Clear Acrylic Hair Clips - 3 Pack",
						  Description = "Transparent acrylic hair clips that work with any hair color. Lightweight and comfortable for all-day wear.",
						  Price = 22.99m,
						  StockQuantity = 35,
						  SKU = "HC-CLEAR-3PK",
						  Material = "Acrylic",
						  Color = "Clear",
						  Size = "Medium",
						  Style = "Hair Clip",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },

                // Decorative Pins
                new Product
					 {
						  Name = "Pearl Decorative Hair Pins - 6 Pack",
						  Description = "Elegant pearl-adorned hair pins perfect for weddings and special occasions. Each pin features a lustrous white pearl accent.",
						  Price = 24.99m,
						  StockQuantity = 40,
						  SKU = "DP-PEARL-6PK",
						  Material = "Metal",
						  Color = "Silver",
						  Size = "Small",
						  Style = "Decorative",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Crystal Flower Hair Pins - 4 Pack",
						  Description = "Sparkling crystal flower hair pins that add glamour to any hairstyle. Perfect for prom, weddings, or date nights.",
						  Price = 32.99m,
						  StockQuantity = 25,
						  SKU = "DP-CRYSTAL-4PK",
						  Material = "Metal",
						  Color = "Silver",
						  Size = "Small",
						  Style = "Decorative",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Vintage Brass Leaf Hair Pins - 5 Pack",
						  Description = "Antique-inspired brass leaf hair pins with intricate detailing. Perfect for bohemian and vintage-style looks.",
						  Price = 28.99m,
						  StockQuantity = 30,
						  SKU = "DP-BRASS-LEAF-5",
						  Material = "Brass",
						  Color = "Brass",
						  Size = "Medium",
						  Style = "Decorative",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },

                // Specialty/Large Clips
                new Product
					 {
						  Name = "Jumbo Hair Claw Clip",
						  Description = "Extra-large claw clip designed for thick or voluminous hair. Strong teeth grip securely without pulling or breaking hair.",
						  Price = 19.99m,
						  StockQuantity = 50,
						  SKU = "CC-JUMBO-BLACK",
						  Material = "Plastic",
						  Color = "Black",
						  Size = "Large",
						  Style = "Hair Clip",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Bamboo Hair Pins - 10 Pack",
						  Description = "Eco-friendly bamboo hair pins for the environmentally conscious. Smooth finish and naturally antimicrobial properties.",
						  Price = 15.99m,
						  StockQuantity = 80,
						  SKU = "BP-BAMBOO-10",
						  Material = "Bamboo",
						  Color = "Natural",
						  Size = "Small",
						  Style = "Bobby Pin",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 },
					 new Product
					 {
						  Name = "Colorful Hair Pins Variety Pack",
						  Description = "Fun assorted pack with pins in multiple colors: red, blue, pink, purple, and green. Great for creative styling and kids.",
						  Price = 11.99m,
						  StockQuantity = 95,
						  SKU = "BP-VARIETY-PACK",
						  Material = "Metal",
						  Color = "Multicolor",
						  Size = "Small",
						  Style = "Bobby Pin",
						  IsActive = true,
						  CreatedDate = DateTime.UtcNow
					 }
				};

		context.Products.AddRange(products);
		await context.SaveChangesAsync();

		// Add sample images for products
		var productImages = new List<ProductImage>();
		var products_with_ids = await context.Products.ToListAsync();

		foreach (var product in products_with_ids)
		{
			// Add primary image
			productImages.Add(new ProductImage
			{
				ProductId = product.Id,
				ImageUrl = $"/images/products/{product.SKU.ToLower()}-main.jpg",
				AltText = $"{product.Name} - Main Image",
				IsPrimary = true,
				DisplayOrder = 1
			});

			// Add secondary image
			productImages.Add(new ProductImage
			{
				ProductId = product.Id,
				ImageUrl = $"/images/products/{product.SKU.ToLower()}-detail.jpg",
				AltText = $"{product.Name} - Detail View",
				IsPrimary = false,
				DisplayOrder = 2
			});

			// Add lifestyle image for decorative items
			if (product.Style == "Decorative")
			{
				productImages.Add(new ProductImage
				{
					ProductId = product.Id,
					ImageUrl = $"/images/products/{product.SKU.ToLower()}-lifestyle.jpg",
					AltText = $"{product.Name} - Lifestyle Image",
					IsPrimary = false,
					DisplayOrder = 3
				});
			}
		}

		context.ProductImages.AddRange(productImages);
		await context.SaveChangesAsync();
	}
}
