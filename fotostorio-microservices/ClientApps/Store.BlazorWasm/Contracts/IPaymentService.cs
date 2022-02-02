using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts;

public interface IPaymentService
{
    Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(Basket basket);
}
