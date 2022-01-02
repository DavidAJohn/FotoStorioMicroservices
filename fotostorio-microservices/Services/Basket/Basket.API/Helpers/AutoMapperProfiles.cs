using AutoMapper;
using Basket.API.Entities;

namespace Basket.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CustomerBasketDTO, CustomerBasket>();
        }
    }
}
