using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HairpinStore.Models;

namespace HairpinStore.Data;

public class ApplicationDbContext : IdentityDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	// Product related DbSets
	public DbSet<Product> Products { get; set; }
	public DbSet<ProductImage> ProductImages { get; set; }

	// Shopping cart DbSets
	public DbSet<Cart> Carts { get; set; }
	public DbSet<CartItem> CartItems { get; set; }

	// Order related DbSets
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Product>(entity =>
		{
			// Product configurations
			entity.HasIndex(e => e.SKU).IsUnique();
			entity.Property(e => e.Price).HasPrecision(18, 2);
		});

		// ProductImage configurations
		builder.Entity<ProductImage>(entity =>
		{
			entity.HasOne(pi => pi.Product)
				.WithMany(p => p.Images)
				.HasForeignKey(pi => pi.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Cart configurations
		builder.Entity<Cart>(entity =>
		{
			entity.HasIndex(e => e.UserId);
		});

		// CartItem configurations
		builder.Entity<CartItem>(entity =>
		{
			entity.HasOne(ci => ci.Cart)
				.WithMany(c => c.Items)
				.HasForeignKey(ci => ci.CartId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(ci => ci.Product)
				.WithMany(p => p.CartItems)
				.HasForeignKey(ci => ci.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
		});

		// Order configurations
		builder.Entity<Order>(entity =>
		{
			entity.HasIndex(e => e.OrderNumber).IsUnique();
			entity.HasIndex(e => e.UserId);
			entity.Property(e => e.SubTotal).HasPrecision(18, 2);
			entity.Property(e => e.ShippingCost).HasPrecision(18, 2);
			entity.Property(e => e.Tax).HasPrecision(18, 2);
			entity.Property(e => e.Total).HasPrecision(18, 2);
		});

		// OrderItem configurations
		builder.Entity<OrderItem>(entity =>
		{
			entity.HasOne(oi => oi.Order)
				.WithMany(o => o.Items)
				.HasForeignKey(oi => oi.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(oi => oi.Product)
				.WithMany(p => p.OrderItems)
				.HasForeignKey(oi => oi.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
			entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
		});
	}
}
