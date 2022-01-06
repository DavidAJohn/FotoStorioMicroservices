using Discount.minAPI.Models;

namespace Discount.minAPI.Data
{
    public interface ICampaignData
    {
        Task<bool> CreateCampaign(Campaign campaign);
        Task<bool> DeleteCampaign(int id);
        Task<IEnumerable<Campaign>> GetAllCampaigns();
        Task<Campaign> GetCampaignById(int id);
        Task<IEnumerable<Campaign>> GetCurrentCampaigns();
        Task<bool> UpdateCampaign(Campaign campaign);
    }
}