using AutoMapper;
using Products.API.DTOs;
using Products.API.Models;

namespace Products.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.Mount, opt => opt.MapFrom(src => src.Mount.Name));

            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>().ReverseMap();
        }
    }
}
