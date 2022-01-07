using AutoMapper;
using Products.Aggregator.Models;

namespace Products.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ProductResponse, AggregatedProduct>();
        }
    }
}
