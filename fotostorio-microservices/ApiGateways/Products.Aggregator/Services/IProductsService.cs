namespace Products.Aggregator.Services;

public interface IProductsService
{
    Task<PagedList<ProductResponse>> GetProductsAsync(ProductParameters productParams);
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<ProductResponse> GetProductBySkuAsync(string sku);
}
