namespace Discount.minAPI.Data;

public class CampaignData : ICampaignData
{
    private readonly ISqlCampaignAccess _sqlCampaignAccess;

    public CampaignData(ISqlCampaignAccess sqlCampaignAccess)
    {
        _sqlCampaignAccess = sqlCampaignAccess;
    }

    public async Task<IEnumerable<Campaign>> GetAllCampaigns() =>
        await _sqlCampaignAccess.GetAllCampaignsAsync();

    public async Task<IEnumerable<Campaign>> GetCurrentCampaigns() =>
        await _sqlCampaignAccess.GetCurrentCampaignsAsync();

    public async Task<Campaign> GetCampaignById(int id) =>
        await _sqlCampaignAccess.GetCampaignByIdAsync(id);

    public async Task<bool> CreateCampaign(Campaign campaign) =>
        await _sqlCampaignAccess.CreateCampaignAsync(campaign);

    public async Task<bool> UpdateCampaign(Campaign campaign) =>
        await _sqlCampaignAccess.UpdateCampaignAsync(campaign);

    public async Task<bool> DeleteCampaign(int id) =>
        await _sqlCampaignAccess.DeleteCampaignAsync(id);
}
