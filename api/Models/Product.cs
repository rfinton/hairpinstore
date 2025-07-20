using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairpinStore.Models;

public class Product
{
	public int Id { get; set; }

	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[Column(TypeName = "decimal(18,2)")]
	public decimal Price { get; set; }

	public int StockQuantity { get; set; }

	[MaxLength(50)]
	public string SKU { get; set; } = string.Empty;

	// Hairpin specific properties
	[MaxLength(50)]
	public string Material { get; set; } = string.Empty;

	[MaxLength(30)]
	public string Color { get; set; } = string.Empty;

	[MaxLength(20)]
	public string Size { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Style { get; set; } = string.Empty;

	public bool IsActive { get; set; } = true;

	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedDate { get; set; }

	// Navigation properties
	public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
	public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

// Product Images
public class ProductImage
{
	public int Id { get; set; }
	public int ProductId { get; set; }

	[Required]
	[MaxLength(500)]
	public string ImageUrl { get; set; } = string.Empty;

	[MaxLength(200)]
	public string AltText { get; set; } = string.Empty;

	public bool IsPrimary { get; set; } = false;
	public int DisplayOrder { get; set; } = 0;

	// Navigation property
	public virtual Product Product { get; set; } = null!;
}

// Shopping cart
public class Cart
{
	public int Id { get; set; }

	[Required]
	public string UserId { get; set; } = string.Empty;

	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	public DateTime LastModified { get; set; } = DateTime.UtcNow;

	// Navigation properties
	public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public class CartItem
{
	public int Id { get; set; }
	public int CartId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal UnitPrice { get; set; }

	public DateTime AddedDate { get; set; } = DateTime.UtcNow;

	// Navigation properties
	public virtual Cart Cart { get; set; } = null!;
	public virtual Product Product { get; set; } = null!;
}

// Orders
public class Order
{
	public int Id { get; set; }

	[Required]
	public string UserId { get; set; } = string.Empty;

	[Required]
	[MaxLength(50)]
	public string OrderNumber { get; set; } = string.Empty;

	public OrderStatus Status { get; set; } = OrderStatus.Pending;

	[Column(TypeName = "decimal(18,2)")]
	public decimal SubTotal { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal ShippingCost { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal Tax { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal Total { get; set; }

	public DateTime OrderDate { get; set; } = DateTime.UtcNow;
	public DateTime? ShippedDate { get; set; }
	public DateTime? DeliveredDate { get; set; }

	// Shipping Address
	[Required]
	[MaxLength(100)]
	public string ShippingName { get; set; } = string.Empty;

	[Required]
	[MaxLength(200)]
	public string ShippingAddress { get; set; } = string.Empty;

	[Required]
	[MaxLength(100)]
	public string ShippingCity { get; set; } = string.Empty;

	[Required]
	[MaxLength(50)]
	public string ShippingState { get; set; } = string.Empty;

	[Required]
	[MaxLength(20)]
	public string ShippingZipCode { get; set; } = string.Empty;

	[Required]
	[MaxLength(50)]
	public string ShippingCounty { get; set; } = string.Empty;

	// Payment info
	[MaxLength(50)]
	public string PaymentMethod { get; set; } = string.Empty;

	[MaxLength(100)]
	public string PaymentTransactionId { get; set; } = string.Empty;

	// Navigation properties
	public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public int ProductId { get; set; }

	[Required]
	[MaxLength(200)]
	public string ProductName { get; set; } = string.Empty;

	public int Quantity { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal UnitPrice { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal TotalPrice { get; set; }

	// Navigation properties
	public virtual Order Order { get; set; } = null!;
	public virtual Product Product { get; set; } = null!;
}

// Enums
public enum OrderStatus
{
	Pending = 0,
	Processing = 1,
	Shipped = 2,
	Delivered = 3,
	Cancelled = 4,
	Refunded = 5
}