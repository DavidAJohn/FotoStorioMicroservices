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

        public async Task<bool> CreateCampaignAsync(Campaign campaign)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var dp = new DynamicParameters();
            dp.Add("@Name", campaign.Name);
            dp.Add("@StartDate", campaign.StartDate);
            dp.Add("@EndDate", campaign.EndDate);

            var sql = @"INSERT INTO [dbo].[Campaigns]
                           ([Name]
                           ,[StartDate]
                           ,[EndDate])
                      VALUES
                           (@Name,
                           @StartDate,
                           @EndDate)";

            var affected = await connection.ExecuteAsync(sql, dp);

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> UpdateCampaignAsync(Campaign campaign)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var dp = new DynamicParameters();
            dp.Add("@Name", campaign.Name);
            dp.Add("@StartDate", campaign.StartDate);
            dp.Add("@EndDate", campaign.EndDate);
            dp.Add("@Id", campaign.Id);

            var sql = @"UPDATE [dbo].[Campaigns] 
                        SET
                           [Name]=@Name,
                           [StartDate]=@StartDate,
                           [EndDate]=@EndDate
                        WHERE [Id]=@Id";

            var affected = await connection.ExecuteAsync(sql, dp);

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteCampaignAsync(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

            var affected = await connection.ExecuteAsync("DELETE FROM [dbo].[Campaigns] WHERE Id = @Id", new { Id = id });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
