using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.API.Contracts;
using Ordering.API.Entities;
using Ordering.API.Extensions;
using Ordering.API.Helpers;
using Ordering.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(ILogger<OrdersController> logger, IOrderRepository orderRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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

            if (string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.PostCode) ||
                string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.City) ||
                string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.County) ||
                string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.Street) ||
                string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.FirstName) ||
                string.IsNullOrWhiteSpace(orderToCreate.SendToAddress.LastName)
                )
            {
                return BadRequest();
            }

            var order = _mapper.Map<Order>(orderToCreate);

            try
            {
                // 1 - use an http client to check jwt validity with identity api

                // 2 - if token is valid, continue and create the order, otherwise return a 401 Not Authorised response

                var email = _httpContextAccessor.HttpContext.GetClaimValueByType("email");
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

        /// GET api/orders/{id}
        /// <summary>
        /// Get an order by Id for an authenticated user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>OrderDetailsDTO</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderByIdForUser(int id)
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.GetClaimValueByType("email");

                var order = await _orderRepository.GetOrderByIdAsync(id, email);

                if (order == null)
                {
                    _logger.LogError($"Order with id: {id}, not found");

                    return NotFound();
                }
                else
                {
                    var orderToReturn = _mapper.Map<Order, OrderDetailsDTO>(order);
                    return orderToReturn;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrderById, from id {id} : {ex.Message}");

                return StatusCode(500, "Internal server error");
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetOrdersForUser()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.GetClaimValueByType("email");

                var orders = await _orderRepository.GetOrdersForUserAsync(email);

                if (orders == null)
                {
                    _logger.LogError($"Orders for user: {email}, not found");

                    return NotFound();
                }
                else
                {
                    return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderDetailsDTO>>(orders));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrdersForUser: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
