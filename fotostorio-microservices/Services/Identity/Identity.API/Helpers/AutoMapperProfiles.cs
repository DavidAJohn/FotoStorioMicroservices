using AutoMapper;
using Identity.API.Models;

namespace Identity.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
        }
    }
}
