using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order = Ordering.API.Models.Order;

namespace Ordering.UnitTests.Controllers;

public class OrdersControllerTests : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _httpContextService;
    private readonly DefaultHttpContext _context;
    private readonly int _fakeOrderId = 1;

    public OrdersControllerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _logger = Substitute.For<ILogger<OrdersController>>();
        _context = new DefaultHttpContext();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _httpContextService = Substitute.For<IHttpContextService>();

        _httpContextAccessor.HttpContext = _context;
    }

    private void SetupHttpContextService()
    {
        _httpContextService
            .GetJwtFromContext(_context)
            .Returns("jwt");

        _httpContextService
            .GetClaimValueByType(_context, "email")
            .Returns("test@test.com");
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnOrder_WhenOrderIsValidAndIsCreatedSuccessfully()
    {
        // Arrange
        SetupHttpContextService();
        var mockToken = _httpContextService.GetJwtFromContext(_context);
        var mockEmail = _httpContextService.GetClaimValueByType(_context, "email");

        Fixture fixture = new Fixture();
        var order = fixture.Create<Order>();
        order.BuyerEmail = mockEmail;

        _orderRepository
            .CreateOrderAsync(Arg.Any<Order>(), Arg.Any<string>())
            .Returns(order);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderCreateDTO>().ReverseMap());
        var mapper = mapperConfig.CreateMapper();

        var orderCreateDTO = fixture.Create<OrderCreateDTO>();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = await sut.CreateOrder(orderCreateDTO);

        //Assert
        result.Should().BeOfType<OkObjectResult>();

        var orderReturned = (result as OkObjectResult)!.Value as Order;
        orderReturned.BuyerEmail.Should().Be(mockEmail);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderToCreateIsNull()
    {
        // Arrange
        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderCreateDTO>().ReverseMap());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (BadRequestResult)await sut.CreateOrder(null!);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderAddressIsIncomplete(string addressProperty)
    {
        // Arrange
        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderCreateDTO>().ReverseMap());
        var mapper = mapperConfig.CreateMapper();

        Fixture fixture = new Fixture();
        var orderCreateDTO = fixture.Create<OrderCreateDTO>();
        orderCreateDTO.SendToAddress.PostCode = addressProperty;

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (BadRequestResult)await sut.CreateOrder(orderCreateDTO);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderCouldNotBeCreated()
    {
        // Arrange
        Fixture fixture = new Fixture();
        var order = fixture.Create<Order>();

        _orderRepository
            .CreateOrderAsync(order, Arg.Any<string>())
            .ReturnsNull();

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderCreateDTO>().ReverseMap());
        var mapper = mapperConfig.CreateMapper();

        var orderCreateDTO = fixture.Create<OrderCreateDTO>();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (BadRequestResult)await sut.CreateOrder(orderCreateDTO);

        //Assert
        result.StatusCode.Should().Be(400);

    }
    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnOrderDetailsDTO_WhenOrderIdExists()
    {
        // Arrange
        SetupHttpContextService();
        var mockToken = _httpContextService.GetJwtFromContext(_context);
        var mockEmail = _httpContextService.GetClaimValueByType(_context, "email");

        Fixture fixture = new Fixture();

        var order = fixture.Create<Order>();
        order.Id = _fakeOrderId;
        order.BuyerEmail = "test@test.com";

        var orderDetailsDTO = fixture.Create<OrderDetailsDTO>();
        orderDetailsDTO.Id = _fakeOrderId;
        orderDetailsDTO.BuyerEmail = "test@test.com";

        _orderRepository
            .GetOrderByIdAsync(_fakeOrderId, mockEmail, mockToken)
            .Returns(order);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetOrderByIdForUser(_fakeOrderId);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<OrderDetailsDTO>();
    }

    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnOrderDetailsDTO_WhenOrderIdExistsAndUserHasAdministratorRole()
    {
        // Arrange
        SetupHttpContextService();
        var mockToken = _httpContextService.GetJwtFromContext(_context);
        var mockEmail = _httpContextService.GetClaimValueByType(_context, "email");

        _httpContextService
            .GetClaimValueByType(_context, "role")
            .Returns("Administrator");

        Fixture fixture = new Fixture();

        var order = fixture.Create<Order>();
        order.Id = _fakeOrderId;
        order.BuyerEmail = "test@test.com";

        var orderDetailsDTO = fixture.Create<OrderDetailsDTO>();
        orderDetailsDTO.Id = _fakeOrderId;
        orderDetailsDTO.BuyerEmail = "test@test.com";

        _orderRepository
            .GetOrderByIdForAdminAsync(_fakeOrderId, mockToken)
            .Returns(order);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetOrderByIdForUser(_fakeOrderId);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<OrderDetailsDTO>();
    }

    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnNotFound_WhenOrderIdDoesNotExist()
    {
        //Arrange
        _orderRepository
            .GetOrderByIdAsync(_fakeOrderId, Arg.Any<string>(), Arg.Any<string>())
            .ReturnsNull();

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (NotFoundResult)await sut.GetOrderByIdForUser(int.MinValue);

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetOrdersForUser_ShouldReturnListOfOrderDetailsDTOs_WhenOrdersExist()
    {
        //Arrange
        SetupHttpContextService();
        var mockToken = _httpContextService.GetJwtFromContext(_context);
        var mockEmail = _httpContextService.GetClaimValueByType(_context, "email");

        Fixture fixture = new Fixture();
        var orderList = fixture.Create<IEnumerable<Order>>();

        _orderRepository
            .GetOrdersForUserAsync(mockEmail, mockToken)
            .Returns(orderList);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetOrdersForUser();

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<OrderDetailsDTO>>();
    }

    [Fact]
    public async Task GetOrdersForUser_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        //Arrange
        _orderRepository
            .GetOrdersForUserAsync(Arg.Any<string>(), Arg.Any<string>())
            .ReturnsNull();

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (NotFoundResult)await sut.GetOrdersForUser();

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetLatestOrders_ShouldReturnListOfOrderDetailsDTO_WhenOrdersExist()
    {
        // Arrange
        SetupHttpContextService();
        var mockToken = _httpContextService.GetJwtFromContext(_context);
        var mockEmail = _httpContextService.GetClaimValueByType(_context, "email");

        Fixture fixture = new Fixture();
        var orderList = fixture.Create<IEnumerable<Order>>();

        _orderRepository
            .GetLatestOrdersAsync(mockToken, Arg.Any<int>())
            .Returns(orderList);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetLatestOrders(int.MinValue);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<OrderDetailsDTO>>();
    }

    [Fact]
    public async Task GetLatestOrders_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        //Arrange
        _orderRepository
            .GetLatestOrdersAsync(Arg.Any<string>(), Arg.Any<int>())
            .ReturnsNull();

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDetailsDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OrdersController(_logger, _orderRepository, mapper, _httpContextAccessor, _httpContextService);
        var result = (NotFoundResult)await sut.GetLatestOrders(int.MinValue);

        //Assert
        result.StatusCode.Should().Be(404);
    }
}
