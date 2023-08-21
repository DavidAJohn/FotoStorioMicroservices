using Inventory.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inventory.UnitTests.Controllers;
public class StockControllerTests : ControllerBase
{
    private readonly ILogger<StockController> _logger;
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _httpContextService;
    private readonly DefaultHttpContext _context;

    public StockControllerTests()
    {
        _stockRepository = Substitute.For<IStockRepository>();
        _logger = Substitute.For<ILogger<StockController>>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _httpContextService = Substitute.For<IHttpContextService>();
        _context = new DefaultHttpContext();
        _httpContextAccessor.HttpContext = _context;
    }

    private void SetupHttpContextService(bool isAdmin = false)
    {
        _httpContextService
            .GetJwtFromContext(_context)
            .Returns("jwt");

        if (isAdmin)
        {
            _httpContextService
                .GetClaimValueByType(_context, "role")
                .Returns("Administrator");
        }
        else
        {
            _httpContextService
                .GetClaimValueByType(_context, "role")
                .Returns("User");
        }
    }

    [Fact]
    public async Task GetStock_ShouldReturnListOfStock_WhenStockExists()
    {
        // Arrange
        var stockList = new List<Stock>
        {
            new Stock
            {
                Sku = "ABC1234567890",
                Name = "Test Product",
                CurrentStock = 10,
                LastUpdated = DateTime.Now
            }
        };

        _stockRepository.ListAllAsync().Returns(stockList);

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (OkObjectResult)await sut.GetStock();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Stock>>();
    }

    [Fact]
    public async Task GetStock_ShouldReturnNotFound_WhenStockDoesNotExist()
    {
        // Arrange
        _stockRepository.ListAllAsync().ReturnsNull();

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (NotFoundResult)await sut.GetStock();

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetStockBySku_ShouldReturnStock_WhenStockForSkuExists()
    {
        // Arrange
        var stockItem = new Stock
        {
            Sku = "ABC1234567890",
            Name = "Test Product",
            CurrentStock = 10,
            LastUpdated = DateTime.Now
        };

        _stockRepository.GetBySkuAsync(Arg.Any<string>()).Returns(stockItem);

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (OkObjectResult)await sut.GetStockBySku(stockItem.Sku);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Stock>();
        result.Value.Should().BeEquivalentTo(stockItem);
    }

    [Fact]
    public async Task GetStockBySku_ShouldReturnBadRequest_WhenSuppliedSkuIsNull()
    {
        // Arrange
        _stockRepository.GetBySkuAsync(Arg.Any<string>()).ReturnsNull();

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (BadRequestResult)await sut.GetStockBySku(null!);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetStockBySku_ShouldReturnNotFound_WhenStockForSkuDoesNotExist()
    {
        // Arrange
        var stockItem = new Stock
        {
            Sku = "ABC1234567890",
            Name = "Test Product",
            CurrentStock = 10,
            LastUpdated = DateTime.Now
        };

        _stockRepository.GetBySkuAsync(Arg.Any<string>()).ReturnsNull();

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (NotFoundResult)await sut.GetStockBySku(stockItem.Sku);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetStockAtOrBelowLevel_ShouldReturnExpectedListOfStock_WhenStockExistsAtOrBelowSuppliedLevel()
    {
        // Arrange
        var stockList = new List<Stock>
        {
            new Stock
            {
                Sku = "ABC1234567890",
                Name = "Test Product 1",
                CurrentStock = 5,
                LastUpdated = DateTime.Now
            },
            new Stock
            {
                Sku = "DEF1234567890",
                Name = "Test Product 2",
                CurrentStock = 10,
                LastUpdated = DateTime.Now
            },
            new Stock
            {
                Sku = "GHJ1234567890",
                Name = "Test Product 3",
                CurrentStock = 3,
                LastUpdated = DateTime.Now
            }
        };

        int requestedStockLevel = 5; // should return a list containing 2 items

        _stockRepository
            .GetByStockLevelAtOrBelow(requestedStockLevel)
            .Returns(stockList.Where(x => x.CurrentStock <= requestedStockLevel).ToList());

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (OkObjectResult)await sut.GetStockAtOrBelowLevel(requestedStockLevel);

        // Assert
        result.StatusCode.Should().Be(200);

        var returnedList = (List<Stock>)result.Value!;
        returnedList.Should().BeOfType<List<Stock>>();
        returnedList.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStockAtOrBelowLevel_ShouldReturnNotFound_WhenNoStockExistsAtOrBelowSuppliedLevel()
    {
        // Arrange
        _stockRepository.GetByStockLevelAtOrBelow(Arg.Any<int>()).ReturnsNull();

        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (NotFoundResult)await sut.GetStockAtOrBelowLevel(int.MinValue);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateNewStockEntry_ShouldReturnCreated_WhenStockEntryCreatedSuccessfully()
    {
        // Arrange
        var stockItem = new Stock
        {
            Sku = "ABC1234567890",
            Name = "Test Product",
            CurrentStock = 10,
            LastUpdated = DateTime.Now
        };

        _stockRepository.Create(Arg.Any<Stock>()).Returns(stockItem);

        SetupHttpContextService(isAdmin: true);
        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (OkObjectResult)await sut.CreateNewStockEntry(stockItem);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Stock>();
        result.Value.Should().BeEquivalentTo(stockItem);
    }

    [Fact]
    public async Task CreateNewStockEntry_ShouldReturnUnauthorized_WhenUserIsNotAdministrator()
    {
        // Arrange
        var stockItem = new Stock
        {
            Sku = "ABC1234567890",
            Name = "Test Product",
            CurrentStock = 10,
            LastUpdated = DateTime.Now
        };

        _stockRepository.Create(Arg.Any<Stock>()).Returns(stockItem);

        SetupHttpContextService(isAdmin: false);
        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (UnauthorizedResult)await sut.CreateNewStockEntry(stockItem);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task CreateNewStockEntry_ShouldReturnBadRequest_WhenStockEntryWasNotCreated()
    {
        // Arrange
        var stockItem = new Stock
        {
            Sku = "ABC1234567890",
            Name = "Test Product",
            CurrentStock = 10,
            LastUpdated = DateTime.Now
        };

        _stockRepository.Create(Arg.Any<Stock>()).ReturnsNull();

        SetupHttpContextService(isAdmin: true);
        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (BadRequestObjectResult)await sut.CreateNewStockEntry(stockItem);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateNewStockEntry_ShouldReturnBadRequest_WhenStockEntryToCreateIsNull()
    {
        // Arrange
        var stockItem = null as Stock;

        _stockRepository.Create(Arg.Any<Stock>()).ReturnsNull();

        SetupHttpContextService(isAdmin: true);
        var sut = new StockController(_logger, _stockRepository, _httpContextAccessor, _httpContextService);

        // Act
        var result = (BadRequestResult)await sut.CreateNewStockEntry(stockItem);

        // Assert
        result.StatusCode.Should().Be(400);
    }
}
