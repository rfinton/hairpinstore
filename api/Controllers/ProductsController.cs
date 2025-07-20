using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HairpinStore.Data;
using HairpinStore.Models;
using System.ComponentModel.DataAnnotations;

namespace HairpinStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public ProductsController(ApplicationDbContext context)
	{
		_context = context;
	}

	// GET: api/products
	[HttpGet]
	public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 4,
		[FromQuery] string? search = null,
		[FromQuery] string? material = null,
		[FromQuery] string? color = null,
		[FromQuery] string? style = null,
		[FromQuery] decimal? minPrice = null,
		[FromQuery] decimal? maxPrice = null,
		[FromQuery] string sortBy = "name",
		[FromQuery] string sortOrder = "asc")
	{
		var query = _context.Products
			.Include(p => p.Images)
			.Where(p => p.IsActive);

		// Apply filters
		if (!string.IsNullOrEmpty(search))
			query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

		if (!string.IsNullOrEmpty(material))
			query = query.Where(p => p.Material.ToLower() == material.ToLower());

		if (!string.IsNullOrEmpty(color))
			query = query.Where(p => p.Color.ToLower() == color.ToLower());

		if (!string.IsNullOrEmpty(style))
			query = query.Where(p => p.Style.ToLower() == style.ToLower());

		if (minPrice.HasValue)
			query = query.Where(p => p.Price <= maxPrice!.Value);

		// Apply sorting
		query = sortBy.ToLower() switch
		{
			"price" => sortOrder.ToLower() == "desc"
				? query.OrderByDescending(p => p.Price)
				: query.OrderBy(p => p.Price),
			"created" => sortOrder.ToLower() == "desc"
				? query.OrderByDescending(p => p.CreatedDate)
				: query.OrderBy(p => p.CreatedDate),
			"stock" => sortOrder.ToLower() == "desc"
				? query.OrderByDescending(p => p.StockQuantity)
				: query.OrderBy(p => p.StockQuantity),
			_ => sortOrder.ToLower() == "desc"
				? query.OrderByDescending(p => p.Name)
				: query.OrderBy(p => p.Name)
		};

		var totalCount = await query.CountAsync();
		var products = await query
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(p => new ProductDto
			{
				Id = p.Id,
				Name = p.Name,
				Description = p.Description,
				Price = p.Price,
				StockQuantity = p.StockQuantity,
				SKU = p.SKU,
				Material = p.Material,
				Color = p.Color,
				Size = p.Size,
				Style = p.Style,
				IsActive = p.IsActive,
				CreatedDate = p.CreatedDate,
				Images = p.Images.OrderBy(img => img.DisplayOrder).Select(img => new ProductImageDto
				{
					Id = img.Id,
					ImageUrl = img.ImageUrl,
					AltText = img.AltText,
					IsPrimary = img.IsPrimary
				}).ToList()
			}).ToListAsync();

		var result = new PagedResult<ProductDto>
		{
			Items = products,
			TotalCount = totalCount,
			Page = pageNumber,
			PageSize = pageSize,
			TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
		};

		return Ok(result);
	}

	// GET: api/products/count
	[HttpGet("count")]
	public async Task<ActionResult<int>> GetProductCount()
	{
		var count = await _context.Products.CountAsync(p => p.IsActive);
		return Ok(count);
	}
	
	// GET: api/products/5
	[HttpGet("{id}")]
	public async Task<ActionResult<ProductDto>> GetProduct(int id)
	{
		var product = await _context.Products
			.Include(p => p.Images)
			.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

		if (product == null)
			return NotFound();

		var productDto = new ProductDto
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			StockQuantity = product.StockQuantity,
			SKU = product.SKU,
			Material = product.Material,
			Color = product.Color,
			Size = product.Size,
			Style = product.Style,
			IsActive = product.IsActive,
			CreatedDate = product.CreatedDate,
			Images = product.Images.OrderBy(img => img.DisplayOrder).Select(img => new ProductImageDto
			{
				Id = img.Id,
				ImageUrl = img.ImageUrl,
				AltText = img.AltText,
				IsPrimary = img.IsPrimary
			}).ToList()
		};

		return Ok(productDto);
	}

	// GET: api/products/filters
	[HttpGet("filters")]
	public async Task<ActionResult<ProductFiltersDto>> GetAvailableFilters()
	{
		var filters = new ProductFiltersDto
		{
			Materials = await _context.Products
				.Where(p => p.IsActive && !string.IsNullOrEmpty(p.Material))
				.Select(p => p.Material)
				.Distinct()
				.OrderBy(m => m)
				.ToListAsync(),
			Colors = await _context.Products
				.Where(p => p.IsActive && !string.IsNullOrEmpty(p.Color))
				.Select(p => p.Color)
				.Distinct()
				.OrderBy(c => c)
				.ToListAsync(),
			Styles = await _context.Products
				.Where(p => p.IsActive && !string.IsNullOrEmpty(p.Style))
				.Select(p => p.Style)
				.Distinct()
				.OrderBy(s => s)
				.ToListAsync(),
			PriceRange = new PriceRangeDto
			{
				MinPrice = (decimal)await _context.Products.Where(p => p.IsActive).MinAsync(p => (double)p.Price),
				MaxPrice = (decimal)await _context.Products.Where(p => p.IsActive).MaxAsync(p => (double)p.Price)
			}
		};

		return Ok(filters);
	}

	// POST: api/products
	[HttpPost]
	[Authorize(Roles = "Administrator,Manager")]
	public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
	{
		// Check if SKU already exists
		if (await _context.Products.AnyAsync(p => p.SKU == createProductDto.SKU))
			return BadRequest("A product with this SKU already exists");

		var product = new Product
		{
			Name = createProductDto.Name,
			Description = createProductDto.Description,
			Price = createProductDto.Price,
			StockQuantity = createProductDto.StockQuantity,
			SKU = createProductDto.SKU,
			Material = createProductDto.Material,
			Color = createProductDto.Color,
			Size = createProductDto.Size,
			Style = createProductDto.Style,
			IsActive = createProductDto.IsActive,
			CreatedDate = DateTime.UtcNow
		};

		_context.Products.Add(product);
		await _context.SaveChangesAsync();

		// Add images if provided
		if (createProductDto.Images != null && createProductDto.Images.Any())
		{
			var images = createProductDto.Images.Select((img, index) => new ProductImage
			{
				ProductId = product.Id,
				ImageUrl = img.ImageUrl,
				AltText = img.AltText,
				IsPrimary = index == 0, // First image is primary
				DisplayOrder = index + 1
			}).ToList();

			_context.ProductImages.AddRange(images);
			await _context.SaveChangesAsync();
		}

		return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, await GetProductDto(product.Id));
	}

	// PUT: api/products/5
	[HttpPut("{id}")]
	[Authorize(Roles = "Administrator,Manager")]
	public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
			return NotFound();

		// Check if SKU change conflicts with existing product
		if (updateProductDto.SKU != product.SKU &&
			await _context.Products.AnyAsync(p => p.SKU == updateProductDto.SKU && p.Id != id))
			return BadRequest("A product with this SKU already exists.");

		product.Name = updateProductDto.Name;
		product.Description = updateProductDto.Description;
		product.Price = updateProductDto.Price;
		product.StockQuantity = updateProductDto.StockQuantity;
		product.SKU = updateProductDto.SKU;
		product.Material = updateProductDto.Material;
		product.Color = updateProductDto.Color;
		product.Size = updateProductDto.Size;
		product.Style = updateProductDto.Style;
		product.IsActive = updateProductDto.IsActive;
		product.UpdatedDate = DateTime.UtcNow;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!await ProductExists(id))
				return NotFound();

			throw;
		}

		return NoContent();
	}

	// PATCH: api/products/5/stock
	[HttpPatch("{id}")]
	[Authorize(Roles = "Adminstrator")]
	public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto updateStockDto)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
			return NotFound();

		product.StockQuantity = updateStockDto.StockQuantity;
		product.UpdatedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync();
		return NoContent();
	}

	// DELETE: api/products/5
	[HttpDelete("{id}")]
	[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> DeleteProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
			return NotFound();

		// Soft delete - just mark as inactive
		product.IsActive = false;
		product.UpdatedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync();
		return NoContent();
	}

	// GET: api/products/low-stock
	[HttpGet("low-stock")]
	[Authorize(Roles = "Administrator,Manager")]
	public async Task<ActionResult<List<ProductDto>>> GetLowStockProducts([FromQuery] int threshold = 10)
	{
		var products = await _context.Products
			.Include(p => p.Images)
			.Where(p => p.IsActive && p.StockQuantity <= threshold)
			.OrderBy(p => p.StockQuantity)
			.Select(p => new ProductDto
			{
				Id = p.Id,
				Name = p.Name,
				Description = p.Description,
				Price = p.Price,
				StockQuantity = p.StockQuantity,
				SKU = p.SKU,
				Material = p.Material,
				Color = p.Color,
				Size = p.Size,
				Style = p.Style,
				IsActive = p.IsActive,
				CreatedDate = p.CreatedDate,
				Images = p.Images.OrderBy(img => img.DisplayOrder).Select(img => new ProductImageDto
				{
					Id = img.Id,
					ImageUrl = img.ImageUrl,
					AltText = img.AltText,
					IsPrimary = img.IsPrimary
				}).ToList()
			}).ToListAsync();

		return Ok(products);
	}

	private async Task<bool> ProductExists(int id)
	{
		return await _context.Products.AnyAsync(e => e.Id == id);
	}

	private async Task<ProductDto> GetProductDto(int id)
	{
		var product = await _context.Products
			.Include(p => p.Images)
			.FirstAsync(p => p.Id == id);

		return new ProductDto
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			StockQuantity = product.StockQuantity,
			SKU = product.SKU,
			Material = product.Material,
			Color = product.Color,
			Size = product.Size,
			Style = product.Style,
			IsActive = product.IsActive,
			CreatedDate = product.CreatedDate,
			Images = product.Images.OrderBy(img => img.DisplayOrder).Select(img => new ProductImageDto
			{
				Id = img.Id,
				ImageUrl = img.ImageUrl,
				AltText = img.AltText,
				IsPrimary = img.IsPrimary
			}).ToList()
		};
	}
}

// DTOs
public class CartDto
{
	public int Id { get; set; }
	public string UserId { get; set; } = string.Empty;
	public IEnumerable<CartItemDto> Items { get; set; } = new List<CartItemDto>();
}

public class CartItemDto
{
	public int ProductId { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int Quantity { get; set; }
	public string Image { get; set; } = string.Empty;

}

public class AddCartItemDto
{
	public int ProductId { get; set; }
	public int Quantity { get; set; }
}

public class RemoveCartItemDto
{
	public int ProductId { get; set; }
}

public class UpdateCartItemDto
{
	public int ProductId { get; set; }
	public int Quantity { get; set; }
}

public class ProductDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockQuantity { get; set; }
	public string SKU { get; set; } = string.Empty;
	public string Material { get; set; } = string.Empty;
	public string Color { get; set; } = string.Empty;
	public string Size { get; set; } = string.Empty;
	public string Style { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public DateTime CreatedDate { get; set; }
	public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
}

public class ProductImageDto
{
	public int Id { get; set; }
	public string ImageUrl { get; set; } = string.Empty;
	public string AltText { get; set; } = string.Empty;
	public bool IsPrimary { get; set; }
}

public class CreateProductDto
{
	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[Required]
	[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
	public decimal Price { get; set; }

	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
	public int StockQuantity { get; set; }

	[Required]
	[MaxLength(50)]
	public string SKU { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Material { get; set; } = string.Empty;

	[MaxLength(30)]
	public string Color { get; set; } = string.Empty;

	[MaxLength(20)]
	public string Size { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Style { get; set; } = string.Empty;

	public bool IsActive { get; set; } = true;

	public List<CreateProductImageDto>? Images { get; set; }
}

public class CreateProductImageDto
{
	[Required]
	[MaxLength(500)]
	public string ImageUrl { get; set; } = string.Empty;

	[MaxLength(200)]
	public string AltText { get; set; } = string.Empty;
}

public class UpdateProductDto
{
	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[Required]
	[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
	public decimal Price { get; set; }

	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
	public int StockQuantity { get; set; }

	[Required]
	[MaxLength(50)]
	public string SKU { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Material { get; set; } = string.Empty;

	[MaxLength(30)]
	public string Color { get; set; } = string.Empty;

	[MaxLength(20)]
	public string Size { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Style { get; set; } = string.Empty;

	public bool IsActive { get; set; }
}

public class UpdateStockDto
{
	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
	public int StockQuantity { get; set; }
}

public class ProductFiltersDto
{
	public List<string> Materials { get; set; } = new List<string>();
	public List<string> Colors { get; set; } = new List<string>();
	public List<string> Styles { get; set; } = new List<string>();
	public PriceRangeDto PriceRange { get; set; } = new PriceRangeDto();
}

public class PriceRangeDto
{
	public decimal MinPrice { get; set; }
	public decimal MaxPrice { get; set; }
}

public class PagedResult<T>
{
	public List<T> Items { get; set; } = new List<T>();
	public int TotalCount { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
}