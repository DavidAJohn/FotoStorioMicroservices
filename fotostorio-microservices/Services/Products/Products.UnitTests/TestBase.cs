using AutoMapper;
using Products.API.Helpers;
using Product = Products.API.Models.Product;

namespace Products.UnitTests;

public class TestBase
{
    protected static IMapper BuildMapper()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapperProfiles());
        });

        return config.CreateMapper();
    }

    protected static Product CreateProductFixture()
    {
        Fixture fixture = new Fixture();
        var product = fixture.Create<Product>();
        product.Id = 1;

        return product;
    }
}
