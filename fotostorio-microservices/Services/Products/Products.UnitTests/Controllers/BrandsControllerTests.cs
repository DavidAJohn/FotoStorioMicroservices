using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Models;

namespace Products.UnitTests.Controllers;
public class BrandsControllerTests
{
    private readonly ILogger<BrandsController> _logger;
    private readonly IBrandRepository _brandRepository;

    public BrandsControllerTests()
    {
        _brandRepository = Substitute.For<IBrandRepository>();
        _logger = Substitute.For<ILogger<BrandsController>>();
    }

    [Fact]
    public async Task GetBrands_ShouldReturnListOfBrands_WhenCalled()
    {
        // Arrange
        var brands = new List<Brand>
        {
            new Brand { Id = 1, Name = "Brand 1" },
            new Brand { Id = 2, Name = "Brand 2" },
            new Brand { Id = 3, Name = "Brand 3" }
        };

        _brandRepository.ListAllAsync().Returns(brands);

        var sut = new BrandsController(_logger, _brandRepository);

        // Act
        var result = (OkObjectResult)await sut.GetBrands();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Brand>>();
        result.Value.Should().BeEquivalentTo(brands);
    }

    [Fact]
    public async Task GetBrandById_ShouldReturnBrand_WhenBrandIdExists()
    {
        // Arrange
        var brand = new Brand { Id = 1, Name = "Brand 1" };

        _brandRepository.GetByIdAsync(Arg.Any<int>()).Returns(brand);

        var sut = new BrandsController(_logger, _brandRepository);

        // Act
        var result = (OkObjectResult)await sut.GetBrandById(brand.Id);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Brand>();
        result.Value.Should().BeEquivalentTo(brand);
    }

    [Fact]
    public async Task GetBrandById_ShouldReturnNotFound_WhenBrandIdDoesNotExist()
    {
        // Arrange
        _brandRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        var sut = new BrandsController(_logger, _brandRepository);

        // Act
        var result = (NotFoundResult)await sut.GetBrandById(int.MinValue);

        // Assert
        result.StatusCode.Should().Be(404);
    }
}
