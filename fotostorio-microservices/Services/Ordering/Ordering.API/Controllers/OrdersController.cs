using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(ILogger<OrdersController> logger, IOrderRepository orderRepository, IMapper mapper)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _mapper = mapper;
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
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDTO orderToCreate)
        {
            if (orderToCreate == null)
            {
                return BadRequest();
            }

            var order = _mapper.Map<Order>(orderToCreate);

            try
            {
                // 1 - use an http client to check jwt validity with identity api

                // 2 - if token is valid, continue and create the order, otherwise return a 401 Not Authorised response

                //var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var email = "dave@test.com";
                order.BuyerEmail = email;

                var createdOrder = await _orderRepository.CreateOrderAsync(order);

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
