using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts;

public interface IAccountService
{
    Task<AddressDTO> GetUserAddressAsync();
    Task<AddressDTO> SaveUserAddressAsync(AddressDTO address);
}
