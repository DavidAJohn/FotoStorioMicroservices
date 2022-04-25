namespace Inventory.API.Contracts;

public interface IUpdateRepository : IBaseRepository<Update>
{
    Task<IEnumerable<Update>> GetBySkuAsync(string sku);
}
