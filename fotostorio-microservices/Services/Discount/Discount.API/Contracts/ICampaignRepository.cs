using Discount.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discount.API.Contracts
{
    public interface ICampaignRepository
    {
        Task<IEnumerable<Campaign>> GetAllCampaignsAsync();
        Task<IEnumerable<Campaign>> GetCurrentCampaignsAsync();
        Task<Campaign> GetCampaignByIdAsync(int id);
        Task<bool> CreateCampaignAsync(Campaign campaign);
        Task<bool> UpdateCampaignAsync(Campaign campaign);
        Task<bool> DeleteCampaignAsync(int id);
    }
}
