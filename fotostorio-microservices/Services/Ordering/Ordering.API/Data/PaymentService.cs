using Stripe;

namespace Ordering.API.Data;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO)
    {
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

        var basket = paymentIntentCreateDTO.Items;

        if (basket == null) return null;

        // calculate basket total
        var basketTotal = (long)0m;

        foreach (var item in basket)
        {
            basketTotal += (long)(item.Total * 100); // supplies amount to Stripe as smallest currency unit (pence)
        }

        // create PaymentIntentService instance
        var service = new PaymentIntentService();

        // create new payment intent by sending basket amount and currency to Stripe
        PaymentIntent intent;

        var intentResult = new PaymentIntentResult { };

        if (string.IsNullOrEmpty(paymentIntentCreateDTO.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = basketTotal, 
                Currency = "gbp",
                PaymentMethodTypes = new List<string> { "card" }
            };

            // await json response from Stripe containing payment intent id and client_secret
            intent = await service.CreateAsync(options);

            if (intent != null)
            {
                intentResult.ClientSecret = intent.ClientSecret;
                intentResult.PaymentIntentId = intent.Id;
            }
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = basketTotal
            };

            // await json response from Stripe containing updated payment intent id and client_secret
            intent = await service.UpdateAsync(paymentIntentCreateDTO.PaymentIntentId, options);

            if (intent != null)
            {
                intentResult.ClientSecret = intent.ClientSecret;
                intentResult.PaymentIntentId = intent.Id;
            }
        }

        if (intentResult != null)
        {
            return intentResult;
        }

        return null;
    }
}
