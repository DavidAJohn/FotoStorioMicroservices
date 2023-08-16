using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Specifications;
using Product = Products.API.Models.Product;

namespace Products.UnitTests.Controllers;

public class ProductsControllerTests : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DefaultHttpContext _context;

    public ProductsControllerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _logger = Substitute.For<ILogger<ProductsController>>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _context = new DefaultHttpContext();
        _httpContextAccessor.HttpContext = _context;
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

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (OkObjectResult)await sut.GetProducts(_productSpecificationParams);

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

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (OkObjectResult)await sut.GetProductById(product.Id);

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

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NotFoundResult)await sut.GetProductById(int.MinValue);

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

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (OkObjectResult)await sut.GetProductBySku(product.Sku);

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

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NotFoundResult)await sut.GetProductBySku("Test_Sku");

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductCreateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (CreatedAtActionResult)await sut.CreateProduct(productCreateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductCreateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (BadRequestResult)await sut.CreateProduct(productCreateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductCreateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (BadRequestResult)await sut.CreateProduct(productCreateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NoContentResult)await sut.UpdateProduct(product.Id, productUpdateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (BadRequestResult)await sut.UpdateProduct(product.Id, productUpdateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NotFoundResult)await sut.UpdateProduct(product.Id, productUpdateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (BadRequestResult)await sut.UpdateProduct(product.Id, productUpdateDTO);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NoContentResult)await sut.DeleteProduct(product.Id);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (NotFoundResult)await sut.DeleteProduct(product.Id);

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

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductUpdateDTO, Product>();
            cfg.CreateMap<Product, ProductDTO>();
        });
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new ProductsController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (BadRequestResult)await sut.DeleteProduct(product.Id);

        //Assert
        result.StatusCode.Should().Be(400);
    }
}
