using AutoMapper;
using Discount.Api.Protos;
using Discount.API.Contracts;
using Discount.API.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Discount.API.Services
{
    public class CampaignService : CampaignProtoService.CampaignProtoServiceBase
    {
        private readonly ICampaignRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public CampaignService(ICampaignRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CampaignModel> GetCampaignById(GetCampaignByIdRequest request, ServerCallContext context)
        {
            var campaign = await _repository.GetCampaignByIdAsync(request.Id);

            if (campaign == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Campaign with Id={request.Id} not found."));
            }

            _logger.LogInformation("Campaign retrieved for Id : {id}", campaign.Id);

            var campaignModel = _mapper.Map<CampaignModel>(campaign);
            return campaignModel;
        }

        public override async Task<CampaignModel> CreateCampaign(CreateCampaignRequest request, ServerCallContext context)
        {
            var campaign = _mapper.Map<Campaign>(request.Campaign);

            await _repository.CreateCampaignAsync(campaign);
            _logger.LogInformation("Campaign was successfully created -> Id : {Id}, Name : {Name}", campaign.Id, campaign.Name);

            var campaignModel = _mapper.Map<CampaignModel>(campaign);
            return campaignModel;
        }

        public override async Task<CampaignModel> UpdateCampaign(UpdateCampaignRequest request, ServerCallContext context)
        {
            var campaign = _mapper.Map<Campaign>(request.Campaign);

            await _repository.UpdateCampaignAsync(campaign);
            _logger.LogInformation("Campaign was successfully updated -> Id : {Id}, Name : {Name}", campaign.Id, campaign.Name);

            var campaignModel = _mapper.Map<CampaignModel>(campaign);
            return campaignModel;
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
}
