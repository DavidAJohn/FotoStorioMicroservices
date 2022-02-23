using System.Threading.Tasks;

namespace Products.API.Contracts
{
    public interface IProductsService
    {
        Task UpdateProductAvailability(string sku, bool status);
    }
}
