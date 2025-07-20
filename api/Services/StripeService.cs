using HairpinStore.Interfaces;
using Stripe;

namespace HairpinStore.Services;

public class StripeService : IStripeService
{
	public async Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency)
	{
		var options = new PaymentIntentCreateOptions
		{
			Amount = amount,
			Currency = currency,
			PaymentMethodTypes = new List<string> { "card" }
		};

		var service = new PaymentIntentService();
		return await service.CreateAsync(options);
	}
}

public class StripeSettings
{
	public string SecretKey { get; set; } = string.Empty;
	public string PublishableKey { get; set; } = string.Empty;
}