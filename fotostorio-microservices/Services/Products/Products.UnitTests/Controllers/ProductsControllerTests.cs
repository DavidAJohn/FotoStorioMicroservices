using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Specifications;
using Product = Products.API.Models.Product;

namespace Products.UnitTests.Controllers;

public class ProductsControllerTests : TestBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DefaultHttpContext _context;
    private ProductsController _sut;

    public ProductsControllerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _logger = Substitute.For<ILogger<ProductsController>>();
        _mapper = BuildMapper();

        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _context = new DefaultHttpContext();
        _httpContextAccessor.HttpContext = _context;

        _sut = new ProductsController(_logger, _productRepository, _mapper, _httpContextAccessor);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnListOfProductDTOs_WhenProductsExist()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var productList = fixture.Create<IEnumerable<Product>>();

        _productRepository
            .ListAllAsync()
            .Returns(productList);

        var _productSpecificationParams = fixture.Create<ProductSpecificationParams>();

        //Act
        var result = (OkObjectResult)await _sut.GetProducts(_productSpecificationParams);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<ProductDTO>>();
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProductDTO_WhenProductExists()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();

        _productRepository
            .GetEntityWithSpecification(Arg.Any<ProductsWithDetailsSpecification>())
            .Returns(product);

        //Act
        var result = (OkObjectResult)await _sut.GetProductById(product.Id);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<ProductDTO>();
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        //Arrange
        _productRepository
            .GetEntityWithSpecification(Arg.Any<ProductsWithDetailsSpecification>())
            .ReturnsNull();

        //Act
        var result = (NotFoundResult)await _sut.GetProductById(int.MinValue);

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProductBySku_ShouldReturnProductDTO_WhenProductExists()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();

        _productRepository
            .GetEntityWithSpecification(Arg.Any<ProductsWithDetailsSpecification>())
            .Returns(product);

        //Act
        var result = (OkObjectResult)await _sut.GetProductBySku(product.Sku);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<ProductDTO>();
    }

    [Fact]
    public async Task GetProductBySku_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        //Arrange
        _productRepository
            .GetEntityWithSpecification(Arg.Any<ProductsWithDetailsSpecification>())
            .ReturnsNull();

        string testSku = "Test_Sku";

        //Act
        var result = (NotFoundResult)await _sut.GetProductBySku(testSku);

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProductDTO_WhenProductIsCreated()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .Create(Arg.Do<Product>(p => product = p))
            .Returns(product);

        var productCreateDTO = fixture.Create<ProductCreateDTO>();

        //Act
        var result = (CreatedAtActionResult)await _sut.CreateProduct(productCreateDTO);

        //Assert
        result.StatusCode.Should().Be(201);
        result.Value.Should().BeOfType<ProductDTO>();
        result.RouteValues!["id"].Should().BeEquivalentTo(product.Id);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenProductCreateDTOIsNull()
    {
        //Arrange
        var productCreateDTO = null as ProductCreateDTO;

        //Act
        var result = (BadRequestResult)await _sut.CreateProduct(productCreateDTO);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenProductCouldNotBeCreated()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .Create(Arg.Do<Product>(p => product = p))
            .ReturnsNull();

        var productCreateDTO = fixture.Create<ProductCreateDTO>();

        //Act
        var result = (BadRequestResult)await _sut.CreateProduct(productCreateDTO);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNoContent_WhenProductIsUpdated()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(product);

        _productRepository
            .Update(Arg.Do<Product>(p => product = p))
            .Returns(true);

        var productUpdateDTO = fixture.Create<ProductUpdateDTO>();

        //Act
        var result = (NoContentResult)await _sut.UpdateProduct(product.Id, productUpdateDTO);

        //Assert
        result.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductUpdateDTOIsNull()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        var productUpdateDTO = null! as ProductUpdateDTO;

        //Act
        var result = (BadRequestResult)await _sut.UpdateProduct(product.Id, productUpdateDTO);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenProductIdDoesNotExist()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        var productUpdateDTO = fixture.Create<ProductUpdateDTO>();

        //Act
        var result = (NotFoundResult)await _sut.UpdateProduct(product.Id, productUpdateDTO);

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductCouldNotBeUpdated()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(product);

        _productRepository
            .Update(Arg.Do<Product>(p => product = p))
            .Returns(false);

        var productUpdateDTO = fixture.Create<ProductUpdateDTO>();

        //Act
        var result = (BadRequestResult)await _sut.UpdateProduct(product.Id, productUpdateDTO);

        //Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenProductIsDeleted()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(product);

        _productRepository
            .Delete(Arg.Do<Product>(p => product = p))
            .Returns(true);

        //Act
        var result = (NoContentResult)await _sut.DeleteProduct(product.Id);

        //Assert
        result.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenProductIdDoesNotExist()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        //Act
        var result = (NotFoundResult)await _sut.DeleteProduct(product.Id);

        //Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnBadRequest_WhenProductIdDoesNotExist()
    {
        //Arrange
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        _productRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(product);

        _productRepository
            .Delete(Arg.Do<Product>(p => product = p))
            .Returns(false);

        //Act
        var result = (BadRequestResult)await _sut.DeleteProduct(product.Id);

        //Assert
        result.StatusCode.Should().Be(400);
    }
}
