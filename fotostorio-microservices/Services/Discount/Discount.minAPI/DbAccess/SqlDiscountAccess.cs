using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Discount.minAPI.DbAccess;

public class SqlDiscountAccess : ISqlDiscountAccess
{
    private readonly IConfiguration _config;

    public SqlDiscountAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<ProductDiscount>> GetAllDiscountsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "GetAllDiscounts";

        var discounts = await connection.QueryAsync<ProductDiscount>(sp, commandType: CommandType.StoredProcedure);

        return discounts;
    }

    public async Task<IEnumerable<ProductDiscount>> GetCurrentDiscountsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "GetCurrentDiscounts";

        var discounts = await connection.QueryAsync<ProductDiscount>(sp, commandType: CommandType.StoredProcedure);

        return discounts;
    }

    public async Task<IEnumerable<ProductDiscount>> GetCurrentAndFutureDiscountsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "GetCurrentAndFutureDiscounts";

        var discounts = await connection.QueryAsync<ProductDiscount>(sp, commandType: CommandType.StoredProcedure);

        return discounts;
    }

    public async Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "GetCurrentDiscountById";

        var dp = new DynamicParameters();
        dp.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

        var discount = await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sp, dp, commandType: CommandType.StoredProcedure);

        if (discount == null)
        {
            return new ProductDiscount
            {
                Sku = "NODISCNT",
                CampaignId = 0,
                SalePrice = 0
            };
        }

        return discount;
    }

    public async Task<ProductDiscount> GetCurrentDiscountBySkuAsync(string sku)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "GetCurrentDiscountBySku";

        var dp = new DynamicParameters();
        dp.Add("@Sku", sku, DbType.String, ParameterDirection.Input);

        var discount =  await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sp, dp, commandType: CommandType.StoredProcedure);

        if (discount == null)
        {
            return new ProductDiscount
            {
                Sku = "NODISCNT",
                CampaignId = 0,
                SalePrice = 0
            };
        }

        return discount;
    }

    public async Task<bool> CreateDiscountAsync(ProductDiscount discount)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "CreateDiscount";

        var dp = new DynamicParameters();
        dp.Add("@Sku", discount.Sku, DbType.String, ParameterDirection.Input);
        dp.Add("@CampaignId", discount.CampaignId, DbType.Int32, ParameterDirection.Input);
        dp.Add("@SalePrice", discount.SalePrice, DbType.Decimal, ParameterDirection.Input);

        var affected = await connection.ExecuteAsync(sp, dp, commandType: CommandType.StoredProcedure);

        if (affected == 0) return false;

        return true;
    }

    public async Task<bool> UpdateDiscountAsync(ProductDiscount discount)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "UpdateDiscount";

        var dp = new DynamicParameters();
        dp.Add("@Sku", discount.Sku, DbType.String, ParameterDirection.Input);
        dp.Add("@CampaignId", discount.CampaignId, DbType.Int32, ParameterDirection.Input);
        dp.Add("@SalePrice", discount.SalePrice, DbType.Decimal, ParameterDirection.Input);
        dp.Add("@Id", discount.Id, DbType.Int32, ParameterDirection.Input);

        var affected = await connection.ExecuteAsync(sp, dp, commandType: CommandType.StoredProcedure);

        if (affected == 0) return false;

        return true;
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sp = "DeleteDiscount";

        var dp = new DynamicParameters();
        dp.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

        var affected = await connection.ExecuteAsync(sp, dp, commandType: CommandType.StoredProcedure);

        if (affected == 0) return false;

        return true;
    }
}
