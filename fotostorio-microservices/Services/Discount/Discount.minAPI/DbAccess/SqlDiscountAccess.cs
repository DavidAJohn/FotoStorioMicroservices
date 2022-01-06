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

        var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                          ,c.[Name] as Campaign
                          ,p.CampaignId as [CampaignId]
                      FROM [ProductDiscounts] p
                      INNER JOIN Campaigns c ON p.CampaignId = c.Id";

        var discounts = await connection.QueryAsync<ProductDiscount>(sql);

        return discounts;
    }
    public async Task<IEnumerable<ProductDiscount>> GetCurrentDiscountsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                          ,c.[Name] as Campaign
                          ,p.CampaignId as [CampaignId]
                      FROM [ProductDiscounts] p
                      INNER JOIN Campaigns c ON p.CampaignId = c.Id
                      WHERE c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE())";

        var discounts = await connection.QueryAsync<ProductDiscount>(sql);

        return discounts;
    }

    public async Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var dp = new DynamicParameters();
        dp.Add("@Id", id);

        var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                          ,[CampaignId]
                          ,c.[Name] as [Campaign]
                      FROM [ProductDiscounts] p
                      INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
                      WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
                      AND p.[Id] = @Id";

        var discount = await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sql, dp);

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
        using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var dp = new DynamicParameters();
        dp.Add("@Sku", sku);

        var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                          ,[CampaignId]
                          ,c.[Name] as [Campaign]
                      FROM [ProductDiscounts] p
                      INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
                      WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
                      AND p.[Sku] = @Sku";

        var discount = await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sql, dp);

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
        using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var dp = new DynamicParameters();
        dp.Add("@Sku", discount.Sku);
        dp.Add("@CampaignId", discount.CampaignId);
        dp.Add("@SalePrice", discount.SalePrice);

        var sql = @"INSERT INTO [dbo].[ProductDiscounts]
                           ([Sku]
                           ,[CampaignId]
                           ,[SalePrice])
                      VALUES
                           (@Sku,
                           @CampaignId,
                           @SalePrice)";

        var affected = await connection.ExecuteAsync(sql, dp);

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<bool> UpdateDiscountAsync(ProductDiscount discount)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var dp = new DynamicParameters();
        dp.Add("@Sku", discount.Sku);
        dp.Add("@CampaignId", discount.CampaignId);
        dp.Add("@SalePrice", discount.SalePrice);
        dp.Add("@Id", discount.Id);

        var sql = @"UPDATE [dbo].[ProductDiscounts] 
                        SET
                           [Sku]=@Sku,
                           [CampaignId]=@CampaignId,
                           [SalePrice]=@SalePrice
                        WHERE [Id]=@Id";

        var affected = await connection.ExecuteAsync(sql, dp);

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var affected = await connection.ExecuteAsync("DELETE FROM [dbo].[ProductDiscounts] WHERE Id = @Id", new { Id = id });

        if (affected == 0)
            return false;

        return true;
    }
}
