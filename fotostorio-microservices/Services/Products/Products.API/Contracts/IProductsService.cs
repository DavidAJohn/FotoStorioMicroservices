using System.Threading.Tasks;

namespace Products.API.Contracts
{
    public interface IProductsService
    {
        Task UpdateProductStockCount(string sku, int quantity);
    }
}
