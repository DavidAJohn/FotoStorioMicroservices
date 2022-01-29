using AutoMapper;
using Ordering.API.Models;

namespace Ordering.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<OrderCreateDTO, Order>();
        }
    }
}
