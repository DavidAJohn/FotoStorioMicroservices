using Microsoft.AspNetCore.Mvc;
using Stripe;
using Order = Ordering.API.Models.Order;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, IOrderRepository orderRepository, IConfiguration config, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _orderRepository = orderRepository;
        _config = config;
        _logger = logger;
    }

    //[Authorize]
    [HttpPost]
    public async Task<ActionResult<PaymentIntentResult>> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO)
    {
        var result = await _paymentService.CreateOrUpdatePaymentIntent(paymentIntentCreateDTO);

        if (result == null) return BadRequest();

        return Ok(result);
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebhook()
    {
        string WhSecret = _config["Stripe:WhSecret"];

        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

        PaymentIntent intent; // Stripe class
        Order order; // application class -> Ordering.API.Models.Order

        switch (stripeEvent.Type)
        {
            case Events.PaymentIntentSucceeded:
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogInformation("Payment Succeeded: {Id}", intent.Id);

                order = await _orderRepository.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentReceived);
                _logger.LogInformation("Order updated to 'payment succeeded': {Id}", order.Id);
                break;

            case Events.PaymentIntentPaymentFailed:
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogInformation("Payment Failed: {Id}", intent.Id);

                order = await _orderRepository.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentFailed);
                _logger.LogInformation("Order updated to 'payment failed': {Id}", order.Id);
                break;
        }

        return new EmptyResult(); // confirms to Stripe that the response has been received
    }
}
