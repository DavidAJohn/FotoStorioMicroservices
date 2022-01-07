using Products.Aggregator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Aggregator.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductResponse>> GetProducts();
    }
}