using AutoMapper;

namespace Ordering.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<OrderCreateDTO, Order>();
        CreateMap<Order, OrderDetailsDTO>();
    }
}
