using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Contracts;
using Ordering.API.Models;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentIntentResult>> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO)
        {
            var result = await _paymentService.CreateOrUpdatePaymentIntent(paymentIntentCreateDTO);

            if (result == null) return BadRequest();

            return Ok(result);
        }
    }
}
