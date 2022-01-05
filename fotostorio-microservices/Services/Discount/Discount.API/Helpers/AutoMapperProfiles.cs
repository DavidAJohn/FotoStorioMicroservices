using AutoMapper;
using Discount.Api.Protos;
using Discount.API.Models;

namespace Discount.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ProductDiscount, DiscountModel>().ReverseMap();
            CreateMap<Campaign, CampaignModel>().ReverseMap();
        }
    }
}
