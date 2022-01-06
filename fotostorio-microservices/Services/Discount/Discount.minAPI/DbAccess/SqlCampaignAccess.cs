using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Discount.minAPI.DbAccess;

public class SqlCampaignAccess : ISqlCampaignAccess
{
    private readonly IConfiguration _config;

    public SqlCampaignAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]";

        var campaigns = await connection.QueryAsync<Campaign>(sql);

        return campaigns;
    }

    public async Task<IEnumerable<Campaign>> GetCurrentCampaignsAsync()
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]
                      WHERE StartDate < GETDATE() AND EndDate > DATEADD(dd,1, GETDATE())";

        var campaigns = await connection.QueryAsync<Campaign>(sql);

        return campaigns;
    }

    public async Task<Campaign> GetCampaignByIdAsync(int id)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var sql = @"SELECT [Id]
                          ,[Name]
                          ,[StartDate]
                          ,[EndDate]
                      FROM [Campaigns]
                      WHERE [Id] = @id";

        var campaign = await connection.QueryFirstOrDefaultAsync<Campaign>(sql, new { Id = id });

        return campaign;
    }

    public async Task<bool> CreateCampaignAsync(Campaign campaign)
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

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
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

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
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DiscountConnectionString"));

        var affected = await connection.ExecuteAsync("DELETE FROM [dbo].[Campaigns] WHERE Id = @Id", new { Id = id });

        if (affected == 0)
            return false;

        return true;
    }
}
