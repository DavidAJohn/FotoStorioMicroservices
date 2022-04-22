using AutoMapper;

namespace Basket.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CustomerBasketDTO, CustomerBasket>();
    }
}
