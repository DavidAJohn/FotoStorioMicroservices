using Discount.Grpc.Protos;
using Discount.Grpc.Contracts;
using Discount.Grpc.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Discount.Grpc.Helpers;

namespace Discount.Grpc.Services;

public class CampaignService : CampaignProtoService.CampaignProtoServiceBase
{
    private readonly ICampaignRepository _repository;
    private readonly ILogger<DiscountService> _logger;

    public CampaignService(ICampaignRepository repository, ILogger<DiscountService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<CampaignModel> GetCampaignById(GetCampaignByIdRequest request, ServerCallContext context)
    {
        var campaign = await _repository.GetCampaignByIdAsync(request.Id);

        if (campaign == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Campaign with Id={request.Id} not found."));
        }

        _logger.LogInformation("Campaign retrieved for Id : {id}", campaign.Id);

        return campaign.ToCampaignModel();
    }

    public override async Task<CampaignModel> CreateCampaign(CreateCampaignRequest request, ServerCallContext context)
    {
        var campaign = request.Campaign.ToCampaign();

        await _repository.CreateCampaignAsync(campaign);
        _logger.LogInformation("Campaign was successfully created -> Id : {Id}, Name : {Name}", campaign.Id, campaign.Name);

        return campaign.ToCampaignModel();
    }

    public override async Task<CampaignModel> UpdateCampaign(UpdateCampaignRequest request, ServerCallContext context)
    {
        var campaign = request.Campaign.ToCampaign();

        await _repository.UpdateCampaignAsync(campaign);
        _logger.LogInformation("Campaign was successfully updated -> Id : {Id}, Name : {Name}", campaign.Id, campaign.Name);

        return campaign.ToCampaignModel();
    }

    public override async Task<DeleteCampaignResponse> DeleteCampaign(DeleteCampaignRequest request, ServerCallContext context)
    {
        var deleted = await _repository.DeleteCampaignAsync(request.Id);

        var response = new DeleteCampaignResponse
        {
            Success = deleted
        };

        _logger.LogInformation("Campaign with Id:{Id} was deleted", request.Id);

        return response;
    }
}
