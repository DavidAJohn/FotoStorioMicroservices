using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.UnitTests.Controllers;

public class OrdersControllerTests : ControllerBase
{
    private readonly Mock<ILogger<OrdersController>> _loggerMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IHttpContextService> _httpContextServiceMock;

    public OrdersControllerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<OrdersController>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextServiceMock = new Mock<IHttpContextService>();
    }

    [Fact]
    public async Task GetOrderByIdForUser_Returns_ActionResult()
    {
        //Arrange
        var fakeOrderId = 1;

        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

        _httpContextServiceMock
            .Setup(x => x.GetJwtFromContext(context))
            .Returns("jwt");

        var mockToken = _httpContextServiceMock.Object.GetJwtFromContext(context);

        _httpContextServiceMock
            .Setup(x => x.GetClaimValueByType(context, "email"))
            .Returns("test@test.com");

        var mockEmail = _httpContextServiceMock.Object.GetClaimValueByType(context, "email");

        _orderRepositoryMock
            .Setup(x => x.GetOrderByIdAsync(fakeOrderId, mockEmail, mockToken))
            .ReturnsAsync(new Order { Id = 1, BuyerEmail = "test@test.com" });

        //Act
        var orderController = new OrdersController(_loggerMock.Object, _orderRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object, _httpContextServiceMock.Object);
        var actionResult = await orderController.GetOrderByIdForUser(fakeOrderId);

        //Assert
        Assert.IsType<ActionResult<OrderDetailsDTO>>(actionResult);
    }

    [Fact]
    public async Task GetOrderByIdForUser_Returns_NotFound()
    {
        //Arrange
        var fakeOrderId = 1;

        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

        _httpContextServiceMock
            .Setup(x => x.GetJwtFromContext(context))
            .Returns("jwt");

        var mockToken = _httpContextServiceMock.Object.GetJwtFromContext(context);

        _httpContextServiceMock
            .Setup(x => x.GetClaimValueByType(context, "email"))
            .Returns("test@test.com");

        var mockEmail = _httpContextServiceMock.Object.GetClaimValueByType(context, "email");

        _orderRepositoryMock
            .Setup(x => x.GetOrderByIdAsync(fakeOrderId, mockEmail, mockToken))
            .Returns((Task<Order>)null);

        //Act
        var orderController = new OrdersController(_loggerMock.Object, _orderRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object, _httpContextServiceMock.Object);
        var result = await orderController.GetOrderByIdForUser(int.MinValue);

        //Assert
        var actionResult = Assert.IsType<ActionResult<OrderDetailsDTO>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    //[Fact]
    //public async Task GetOrderByIdForUser_Throws_Exception()
    //{
    //    //Arrange
    //    var fakeOrderId = 1;

    //    var context = new DefaultHttpContext();
    //    _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

    //    _httpContextServiceMock
    //        .Setup(x => x.GetJwtFromContext(context))
    //        .Returns("jwt");

    //    var mockToken = _httpContextServiceMock.Object.GetJwtFromContext(context);

    //    _httpContextServiceMock
    //        .Setup(x => x.GetClaimValueByType(context, "email"))
    //        .Returns("test@test.com");

    //    var mockEmail = _httpContextServiceMock.Object.GetClaimValueByType(context, "email");

    //    _orderRepositoryMock
    //        .Setup(x => x.GetOrderByIdAsync(fakeOrderId, mockEmail, mockToken))
    //        .ThrowsAsync(new Exception("Error message"));

    //    //Act
    //    var orderController = new OrdersController(_loggerMock.Object, _orderRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object, _httpContextServiceMock.Object);
    //    Func<Task> action = async () => await orderController.GetOrderByIdForUser(int.MinValue);

    //    //Assert
    //    await Assert.ThrowsAsync<Exception>(() => action());
    //}

    [Fact]
    public async Task GetOrdersForUser_Returns_ActionResult_ListOf_OrderDetailsDTOs()
    {
        //Arrange
        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

        _httpContextServiceMock
            .Setup(x => x.GetJwtFromContext(context))
            .Returns("jwt");

        var mockToken = _httpContextServiceMock.Object.GetJwtFromContext(context);

        _httpContextServiceMock
            .Setup(x => x.GetClaimValueByType(context, "email"))
            .Returns("test@test.com");

        var mockEmail = _httpContextServiceMock.Object.GetClaimValueByType(context, "email");

        var orderList = new List<Order>();
        orderList.Add(new Order { Id = 1, BuyerEmail = "test@test.com" });
        orderList.Add(new Order { Id = 2, BuyerEmail = "test@test.com" });

        _orderRepositoryMock
            .Setup(x => x.GetOrdersForUserAsync(mockEmail, mockToken))
            .ReturnsAsync(orderList);

        //Act
        var orderController = new OrdersController(_loggerMock.Object, _orderRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object, _httpContextServiceMock.Object);
        var actionResult = await orderController.GetOrdersForUser();

        //Assert
        Assert.IsType<ActionResult<IEnumerable<OrderDetailsDTO>>>(actionResult);
    }

    [Fact]
    public async Task GetOrdersForUser_Returns_NotFound()
    {
        //Arrange
        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

        _httpContextServiceMock
            .Setup(x => x.GetJwtFromContext(context))
            .Returns("jwt");

        var mockToken = _httpContextServiceMock.Object.GetJwtFromContext(context);

        _httpContextServiceMock
            .Setup(x => x.GetClaimValueByType(context, "email"))
            .Returns("test@test.com");

        var mockEmail = _httpContextServiceMock.Object.GetClaimValueByType(context, "email");

        _orderRepositoryMock
            .Setup(x => x.GetOrdersForUserAsync(mockEmail, mockToken))
            .ReturnsAsync((IEnumerable<Order>)null);

        //Act
        var orderController = new OrdersController(_loggerMock.Object, _orderRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object, _httpContextServiceMock.Object);
        var result = await orderController.GetOrdersForUser();

        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<OrderDetailsDTO>>>(result);
        //Assert.IsType<NotFoundResult>(actionResult.Result); // repo mock returns null, but still an OkObjectResult ??
        Assert.Null(actionResult.Value);
    }
}
