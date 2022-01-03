using Discount.API.Contracts;
using Discount.API.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Discount.API.Data
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly IConfiguration _config;

        public CampaignRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]";

            var campaigns = await connection.QueryAsync<Campaign>(sql);

            return campaigns;
        }

        public async Task<Campaign> GetCampaignByIdAsync(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]
                      WHERE [Id] = @id";

            var campaign = await connection.QueryFirstOrDefaultAsync<Campaign>(sql, new { Id = id });

            return campaign;
        }

        public async Task<IEnumerable<Campaign>> GetCurrentCampaignsAsync()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]
                      WHERE StartDate < GETDATE() AND EndDate > DATEADD(dd,1, GETDATE())";

            var campaigns = await connection.QueryAsync<Campaign>(sql);

            return campaigns;
        }

        public Task<bool> CreateCampaignAsync(Campaign campaign)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateCampaignAsync(Campaign campaign)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteCampaignAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
