using AutoMapper;

namespace Identity.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Address, AddressDTO>().ReverseMap();
    }
}
