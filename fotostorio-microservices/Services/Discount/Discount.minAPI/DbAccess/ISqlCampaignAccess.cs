using Discount.minAPI.Models;

namespace Discount.minAPI.DbAccess;

public interface ISqlCampaignAccess
{
    Task<bool> CreateCampaignAsync(Campaign campaign);
    Task<bool> DeleteCampaignAsync(int id);
    Task<IEnumerable<Campaign>> GetAllCampaignsAsync();
    Task<Campaign> GetCampaignByIdAsync(int id);
    Task<IEnumerable<Campaign>> GetCurrentCampaignsAsync();
    Task<bool> UpdateCampaignAsync(Campaign campaign);
}
