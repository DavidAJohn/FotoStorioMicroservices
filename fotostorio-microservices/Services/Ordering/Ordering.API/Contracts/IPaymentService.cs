using Ordering.API.Models;
using System.Threading.Tasks;

namespace Ordering.API.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO);
    }
}
