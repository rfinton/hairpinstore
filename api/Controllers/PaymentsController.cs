using HairpinStore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace HairpinStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
	private readonly IStripeService _stripeService;

	public PaymentsController(IStripeService stripeService)
	{
		_stripeService = stripeService;
	}

	[HttpPost("create-payment-intent")]
	public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
	{
		var intent = await _stripeService.CreatePaymentIntentAsync(request.Amount, request.Currency);
		return Ok(new { clientSecret = intent.ClientSecret });
	}
}

public class PaymentIntentRequest
{
	public long Amount { get; set; }
	public string Currency { get; set; } = "usd";
}