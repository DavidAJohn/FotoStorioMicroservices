using Inventory.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Inventory.UnitTests.Controllers;
public class UpdatesControllerTests : ControllerBase
{
    private readonly ILogger<UpdatesController> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _httpContextService;
    private readonly DefaultHttpContext _context;

    public UpdatesControllerTests()
    {
        _logger = Substitute.For<ILogger<UpdatesController>>();
        _inventoryService = Substitute.For<IInventoryService>();
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
    public async Task GetUpdates_ShouldReturnListOfUpdates_WhenUpdatesExistAndUserIsAdministrator()
    {
        // Arrange
        var updatesList = new List<Update>
        {
            new Update
            {
                Id = 1,
                Sku = "ABC1234567890",
                UpdatedAt = DateTime.Now,
                Added = 10,
                Removed = 0
            }
        };

        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.GetUpdates(token).Returns(updatesList);

        // Act
        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetUpdates();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Update>>();
        result.Value.Should().BeEquivalentTo(updatesList);
    }

    [Fact]
    public async Task GetUpdates_ShouldReturnUnauthorized_WhenUserIsNotAdministrator()
    {
        // Arrange
        SetupHttpContextService(isAdmin: false);

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (UnauthorizedResult)await sut.GetUpdates();

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetUpdates_ShouldReturnNotFound_WhenNoUpdatesExist()
    {
        // Arrange
        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.GetUpdates(token).ReturnsNull();

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (NotFoundResult)await sut.GetUpdates();

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetUpdatesBySku_ShouldReturnListOfUpdates_WhenUpdatesExistAndUserIsAdministrator()
    {
        // Arrange
        var updatesList = new List<Update>
        {
            new Update
            {
                Id = 1,
                Sku = "ABC1234567890",
                UpdatedAt = DateTime.Now,
                Added = 10,
                Removed = 0
            }
        };

        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.GetUpdatesBySku(updatesList.First().Sku, token).Returns(updatesList);

        // Act
        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.GetUpdatesBySku(updatesList.First().Sku);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Update>>();
        result.Value.Should().BeEquivalentTo(updatesList);
    }

    [Fact]
    public async Task GetUpdatesBySku_ShouldReturnUnauthorized_WhenUserIsNotAdministrator()
    {
        // Arrange
        SetupHttpContextService(isAdmin: false);

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (UnauthorizedResult)await sut.GetUpdatesBySku("ABC1234567890");

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetUpdatesBySku_ShouldReturnNotFound_WhenNoUpdatesExist()
    {
        // Arrange
        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.GetUpdatesBySku(Arg.Any<string>(), token).ReturnsNull();

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (NotFoundResult)await sut.GetUpdatesBySku("ABC1234567890");

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetUpdatesBySku_ShouldReturnBadRequest_WhenSuppliedSkuIsNull()
    {
        // Arrange
        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (BadRequestResult)await sut.GetUpdatesBySku(null!);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateStockUpdate_ShouldReturnCreatedUpdate_WhenUpdateIsValidAndUserIsAdmin()
    {
        // Arrange
        var updateToCreate = new UpdateCreateDTO
        {
            Sku = "ABC1234567890",
            Added = 10,
            Removed = 0
        };

        var createdUpdate = new Update
        {
            Id = 1,
            Sku = "ABC1234567890",
            UpdatedAt = DateTime.Now,
            Added = 10,
            Removed = 0
        };

        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.CreateUpdateFromAdmin(Arg.Any<UpdateCreateDTO>(), token).Returns(createdUpdate);

        // Act
        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);
        var result = (OkObjectResult)await sut.CreateStockUpdate(updateToCreate);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Update>();
        result.Value.Should().BeEquivalentTo(createdUpdate);
    }

    [Fact]
    public async Task CreateStockUpdate_ShouldReturnUnauthorized_WhenUserIsNotAdministrator()
    {
        // Arrange
        var updateToCreate = new UpdateCreateDTO
        {
            Sku = "ABC1234567890",
            Added = 10,
            Removed = 0
        };

        SetupHttpContextService(isAdmin: false);

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (UnauthorizedResult)await sut.CreateStockUpdate(updateToCreate);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task CreateStockUpdate_ShouldReturnBadRequest_WhenUpdateIsNull()
    {
        // Arrange
        var updateToCreate = null as UpdateCreateDTO;

        SetupHttpContextService(isAdmin: true);

        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);

        // Act
        var result = (BadRequestResult)await sut.CreateStockUpdate(updateToCreate);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateStockUpdate_ShouldReturnBadRequest_WhenUpdateWasNotCreated()
    {
        // Arrange
        var updateToCreate = new UpdateCreateDTO
        {
            Sku = "ABC1234567890",
            Added = 10,
            Removed = 0
        };

        SetupHttpContextService(isAdmin: true);
        var token = _httpContextService.GetJwtFromContext(_context);

        _inventoryService.CreateUpdateFromAdmin(Arg.Any<UpdateCreateDTO>(), token).ReturnsNull();

        // Act
        var sut = new UpdatesController(_logger, _inventoryService, _httpContextAccessor, _httpContextService);
        var result = (BadRequestObjectResult)await sut.CreateStockUpdate(updateToCreate);

        // Assert
        result.StatusCode.Should().Be(400);
    }
}
