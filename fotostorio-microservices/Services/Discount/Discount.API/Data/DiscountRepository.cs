using Discount.API.Contracts;
using Discount.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Discount.API.Data
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _config;

        public DiscountRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<ProductDiscount>> GetAllDiscountsAsync()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                          ,c.[Name] as CampaignName
                      FROM [ProductDiscounts] p
                      INNER JOIN Campaigns c ON p.CampaignId = c.Id";

            var discounts = await connection.QueryAsync<ProductDiscount>(sql);

            return discounts;
        }

        public async Task<IEnumerable<ProductDiscount>> GetCurrentDiscountsAsync()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                      FROM [ProductDiscounts] p
                      INNER JOIN Campaigns c ON p.CampaignId = c.Id
                      WHERE c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE())";

            var discounts = await connection.QueryAsync<ProductDiscount>(sql);

            return discounts;
        }

        public async Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                      FROM [ProductDiscounts] p
                      INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
                      WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
                      AND p.[Id] = @id";

            var discount = await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sql, new { Id = id });

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

            var sql = @"SELECT p.[Id]
                          ,[Sku]
                          ,[SalePrice]
                      FROM [ProductDiscounts] p
                      INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
                      WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
                      AND p.[Sku] = @sku";

            var discount = await connection.QueryFirstOrDefaultAsync<ProductDiscount>(sql, new { Sku = sku });

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

        public Task<bool> CreateDiscountAsync(ProductDiscount discount)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateDiscountAsync(ProductDiscount discount)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteDiscountAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
