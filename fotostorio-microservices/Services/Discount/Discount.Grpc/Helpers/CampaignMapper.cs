using Discount.Grpc.Models;
using Discount.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace Discount.Grpc.Helpers;

public static class CampaignMapper
{
    public static CampaignModel ToCampaignModel(this Campaign campaign)
    {
        return new CampaignModel
        {
            Id = campaign.Id,
            Name = campaign.Name,
            StartDate = Timestamp.FromDateTime(campaign.StartDate),
            EndDate = Timestamp.FromDateTime(campaign.EndDate)
        };
    }

    public static Campaign ToCampaign(this CampaignModel campaignModel)
    {
        return new Campaign
        {
            Id = campaignModel.Id,
            Name = campaignModel.Name,
            StartDate = campaignModel.StartDate.ToDateTime(),
            EndDate = campaignModel.EndDate.ToDateTime()
        };
    }
}
