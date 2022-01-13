using AutoMapper;
using Discount.Grpc.Protos;
using Discount.Grpc.Models;

namespace Discount.Grpc.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ProductDiscount, DiscountModel>().ReverseMap();
        CreateMap<Campaign, CampaignModel>().ReverseMap();
    }
}
