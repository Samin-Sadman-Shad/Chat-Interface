using MediatR;
using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Repositories;
using Chat.Interface.Server.Infrastructure.Services;

namespace Chat.Interface.Server.Application.Commands;

public record GenerateCampaignCommand(
    string Prompt,
    List<Guid> DataSourceIds,
    List<ChannelType> PreferredChannels
) : IRequest<Campaign>;

public class GenerateCampaignCommandHandler : IRequestHandler<GenerateCampaignCommand, Campaign>
{
    private readonly ICampaignGenerator _campaignGenerator;
    private readonly IDataSourceRepository _dataSourceRepository;
    private readonly ICampaignRepository _campaignRepository;

    public GenerateCampaignCommandHandler(
        ICampaignGenerator campaignGenerator,
        IDataSourceRepository dataSourceRepository,
        ICampaignRepository campaignRepository)
    {
        _campaignGenerator = campaignGenerator;
        _dataSourceRepository = dataSourceRepository;
        _campaignRepository = campaignRepository;
    }

    public async Task<Campaign> Handle(GenerateCampaignCommand request, CancellationToken cancellationToken)
    {
        var dataSources = await _dataSourceRepository.GetByIdsAsync(request.DataSourceIds);
        var campaign = await _campaignGenerator.GenerateAsync(request.Prompt, dataSources, request.PreferredChannels);
        
        await _campaignRepository.AddAsync(campaign);
        return campaign;
    }
}