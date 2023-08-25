using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Models;

namespace Products.UnitTests.Controllers;

public class CategoriesControllerTests : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesControllerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _logger = Substitute.For<ILogger<CategoriesController>>();
    }

    [Fact]
    public async Task GetCategories_ShouldReturnListOfCategories_WhenCalled()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" },
            new Category { Id = 3, Name = "Category 3" }
        };

        _categoryRepository.ListAllAsync().Returns(categories);

        var sut = new CategoriesController(_logger, _categoryRepository);

        // Act
        var result = (OkObjectResult)await sut.GetCategories();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Category>>();
        result.Value.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategory_WhenCategoryIdExists()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category 1" };

        _categoryRepository.GetByIdAsync(Arg.Any<int>()).Returns(category);

        var sut = new CategoriesController(_logger, _categoryRepository);

        // Act
        var result = (OkObjectResult)await sut.GetCategoryById(category.Id);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Category>();
        result.Value.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenCategoryIdDoesNotExist()
    {
        // Arrange
        _categoryRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        var sut = new CategoriesController(_logger, _categoryRepository);

        // Act
        var result = (NotFoundResult)await sut.GetCategoryById(int.MinValue);

        // Assert
        result.StatusCode.Should().Be(404);
    }
}
