using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.API.Contracts;
using Ordering.API.Entities;
using Ordering.API.Models;
using System;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrdersController(ILogger<OrdersController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        /// POST api/orders
        /// <summary>
        /// Creates a new customer order
        /// </summary>
        /// <returns>Order</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            try
            {
                //var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var email = "";
                var basket = new Basket { };
                var address = new Address { };

                var createdOrder = await _orderRepository.CreateOrderAsync(email, basket, address);

                if (createdOrder == null) return BadRequest();

                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateOrder : {ex.Message}");

                return BadRequest();
            }
        }
    }
}
