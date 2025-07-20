using Stripe;

namespace HairpinStore.Interfaces;

public interface IStripeService
{
	Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency);
}
