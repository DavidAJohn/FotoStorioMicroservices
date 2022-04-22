using AutoMapper;

namespace Products.Aggregator.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ProductResponse, AggregatedProduct>();
    }
}
