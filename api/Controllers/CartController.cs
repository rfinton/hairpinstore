using System.Security.Claims;
using HairpinStore.Data;
using HairpinStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HairpinStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public CartController(ApplicationDbContext context)
	{
		_context = context;
	}

	// GET: api/cart
	[HttpGet]
	public async Task<ActionResult<CartDto>> GetCart()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized("User not authenticated");

		var cart = await _context.Carts
			.Include(c => c.Items)
			.ThenInclude(i => i.Product)
			.ThenInclude(p => p.Images)
			.FirstOrDefaultAsync(c => c.UserId == userId);

		if (cart == null)
			return NotFound("Cart not found");

		return Ok(new CartDto
		{
			Id = cart.Id,
			UserId = userId,
			Items = cart.Items.Select(item => new CartItemDto
			{
				ProductId = item.Product.Id,
				Name = item.Product.Name,
				Price = item.Product.Price,
				Quantity = item.Quantity,
				Image = item.Product.Images.First(i => i.IsPrimary).ImageUrl
			})
		});
	}

	// POST: api/cart/add
	[HttpPost("add")]
	public async Task<ActionResult<CartItemDto>> AddItemToCart([FromBody] AddCartItemDto dto)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized("User not authenticated");

		var cart = await _context.Carts
			.Include(c => c.Items)
			.ThenInclude(i => i.Product.Images)
			.FirstOrDefaultAsync(c => c.UserId == userId);

		if (cart == null)
		{
			cart = new Cart { UserId = userId, Items = new List<CartItem>() };
			_context.Carts.Add(cart);
		}

		var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
		if (existingItem != null)
		{
			existingItem.Quantity += dto.Quantity;
		}
		else
		{
			var product = await _context.Products
				.Include(p => p.Images)
				.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
				
			if (product == null)
				return NotFound("Item not found");

			cart.Items.Add(new CartItem
			{
				ProductId = product.Id,
				Quantity = dto.Quantity,
				UnitPrice = product.Price
			});
		}

		await _context.SaveChangesAsync();

		return Ok(new CartDto
		{
			Id = cart.Id,
			UserId = userId,
			Items = cart.Items.Select(item => new CartItemDto
			{
				ProductId = item.Product.Id,
				Name = item.Product.Name,
				Price = item.Product.Price,
				Quantity = item.Quantity,
				Image = item.Product.Images.First(i => i.IsPrimary).ImageUrl
			})
		});
	}

	// PUT: api/cart/update
	[HttpPut("update")]
	public async Task<ActionResult<CartDto>> UpdateItemQuantity([FromBody] UpdateCartItemDto dto)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized("User not authenticated");

		var cart = await _context.Carts
			.Include(c => c.Items)
			.ThenInclude(i => i.Product.Images)
			.FirstOrDefaultAsync(c => c.UserId == userId);

		if (cart == null)
			return NotFound("Cart not found");

		var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
		if (item == null)
			return NotFound("Item not found in cart");

		item.Quantity = dto.Quantity;

		await _context.SaveChangesAsync();

		return Ok(new CartDto
		{
			Id = cart.Id,
			UserId = userId,
			Items = cart.Items.Select(item => new CartItemDto
			{
				ProductId = item.Product.Id,
				Name = item.Product.Name,
				Price = item.Product.Price,
				Quantity = item.Quantity,
				Image = item.Product.Images.First(i => i.IsPrimary).ImageUrl
			})
		});
	}

	// DELETE: api/cart/remove
	[HttpDelete("remove")]
	public async Task<ActionResult<CartItemDto>> RemoveItemFromCart([FromBody] RemoveCartItemDto dto)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized("User not authenticated");

		var cart = await _context.Carts
			.Include(c => c.Items)
			.ThenInclude(i => i.Product.Images)
			.FirstOrDefaultAsync(c => c.UserId == userId);

		if (cart == null)
			return NotFound("Cart not found");

		var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
		if (item == null)
			return NotFound("Item not found in cart");

		cart.Items.Remove(item);
		await _context.SaveChangesAsync();

		return Ok(new CartDto
		{
			Id = cart.Id,
			UserId = userId,
			Items = cart.Items.Select(item => new CartItemDto
			{
				ProductId = item.Product.Id,
				Name = item.Product.Name,
				Price = item.Product.Price,
				Quantity = item.Quantity,
				Image = item.Product.Images.First(i => i.IsPrimary).ImageUrl
			})
		});
	}

	// DELETE: api/cart/clear
	[HttpDelete("clear")]
	public async Task<ActionResult> ClearCart()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized("User not authenticated");

		var cart = await _context.Carts
			.Include(c => c.Items)
			.FirstOrDefaultAsync(c => c.UserId == userId);

		if (cart == null)
			return NotFound("Cart not found");

		cart.Items.Clear();
		await _context.SaveChangesAsync();

		return Ok(cart);
	}
}
