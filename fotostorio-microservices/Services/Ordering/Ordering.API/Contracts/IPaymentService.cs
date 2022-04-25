namespace Ordering.API.Contracts;

public interface IPaymentService
{
    Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO);
}
