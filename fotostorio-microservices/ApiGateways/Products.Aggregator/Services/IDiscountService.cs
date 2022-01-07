using Products.Aggregator.Models;
using System.Threading.Tasks;

namespace Products.Aggregator.Services
{
    public interface IDiscountService
    {
        Task<DiscountResponse> GetDiscountBySku(string sku);
    }
}