using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _contextService;

    public OrdersController(ILogger<OrdersController> logger, IOrderRepository orderRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHttpContextService contextService)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _contextService = contextService;
    }

    /// POST api/orders
    /// <summary>
    /// Creates a new customer order
    /// </summary>
    /// <returns>Order</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(OrderCreateDTO orderToCreate)
    {
        if (orderToCreate == null)
        {
            _logger.LogWarning("Attempt to create new order failed: OrderCreateDTO object was null");
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.PostCode) ||
            string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.City) ||
            string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.County) ||
            string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.Street) ||
            string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.FirstName) ||
            string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.LastName)
            )
        {
            _logger.LogWarning("Attempt to create new order failed: {@OrderCreateDTO} object failed validation", orderToCreate);
            return BadRequest();
        }

        var order = _mapper.Map<Order>(orderToCreate);

        var token = _contextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var email = _contextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "email");
        order.BuyerEmail = email;

        var createdOrder = await _orderRepository.CreateOrderAsync(order, token);

        if (createdOrder == null)
        {
            _logger.LogError("Order creation failed : {@OrderCreateDTO}", orderToCreate);
            return BadRequest();
        }

        return Ok(createdOrder);
    }

    /// GET api/orders/{id}
    /// <summary>
    /// Get an order by Id for an authenticated user
    /// </summary>
    /// <param name="id"></param>
    /// <returns>OrderDetailsDTO</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderByIdForUser(int id)
    {
        var token = _contextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var email = _contextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "email");
        var role = _contextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "role");

        Order order = new Order {};

        if (role == "Administrator" || role == "Marketing")
        {
            order = await _orderRepository.GetOrderByIdForAdminAsync(id, token);
        }
        else
        {
            order = await _orderRepository.GetOrderByIdAsync(id, email, token);
        }
            
        if (order == null)
        {
            _logger.LogError("Order with id: {orderId}, not found", id);

            return NotFound();
        }
        else
        {
            var orderToReturn = _mapper.Map<Order, OrderDetailsDTO>(order);
            return Ok(orderToReturn);
        }
    }

    /// GET api/orders/
    /// <summary>
    /// Get existing orders for an authenticated user
    /// </summary>
    /// <returns>IEnumerable<OrderDetailsDTO></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var token = _contextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        if (token == null) { return NotFound(); }

        var email = _contextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "email");

        var orders = await _orderRepository.GetOrdersForUserAsync(token, email);
        _logger.LogInformation("Request for previous orders for: {email}", email);

        if (orders == null)
        {
            _logger.LogInformation("No orders found for user: {email}", email);

            return Ok(new List<OrderDetailsDTO>() { });
        }
        else
        {
            return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderDetailsDTO>>(orders));
        }
    }

    /// GET api/orders/latest/{int}
    /// <summary>
    /// Get orders since no. of {days}
    /// </summary>
    /// <returns>IEnumerable<OrderDetailsDTO></returns>
    [HttpGet("latest/{days:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestOrders(int days)
    {
        var token = _contextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        if (token == null) { return NotFound(); }

        var email = _contextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "email");

        var orders = await _orderRepository.GetLatestOrdersAsync(token, days);
        _logger.LogInformation("Request for latest orders made by: {email}", email);

        if (orders == null)
        {
            _logger.LogInformation("Request for orders in the last {numOfDays} days - returned null", days);

            return Ok(new List<OrderDetailsDTO>(){});
        }
        else
        {
            return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderDetailsDTO>>(orders));
        }
    }
}
