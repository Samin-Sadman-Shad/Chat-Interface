using Microsoft.AspNetCore.Mvc;
using MediatR;
using Chat.Interface.Server.Application.Commands;
using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Services;

namespace Chat.Interface.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICampaignExecutionService _executionService;

    public CampaignController(IMediator mediator, ICampaignExecutionService executionService)
    {
        _mediator = mediator;
        _executionService = executionService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<Campaign>> GenerateCampaign([FromBody] GenerateCampaignRequest request)
    {
        var command = new GenerateCampaignCommand(request.Prompt, request.DataSourceIds, request.PreferredChannels);
        var campaign = await _mediator.Send(command);
        return Ok(campaign);
    }

    [HttpPost("{id}/execute")]
    public async Task<ActionResult<object>> GetExecutionPayload(Guid id)
    {
        // In a real implementation, you'd fetch the campaign from the repository
        var campaign = new Campaign
        {
            Id = id,
            Name = "Sample Campaign",
            Message = "Limited time offer - Get 20% off your next purchase!",
            Channels = new List<ChannelType> { ChannelType.Email, ChannelType.SMS },
            Audience = new AudienceSegment
            {
                Name = "High-Value Customers",
                EstimatedSize = 10000,
                Criteria = new Dictionary<string, object>
                {
                    ["ageRange"] = "25-45",
                    ["location"] = "US",
                    ["behavior"] = "purchased_last_90_days"
                }
            },
            Timing = new CampaignTiming
            {
                OptimalTime = DateTime.UtcNow.AddDays(1),
                TimeZone = "UTC",
                PreferredDays = new List<string> { "Monday", "Tuesday", "Wednesday" }
            }
        };

        var executionPayload = await _executionService.GenerateExecutionPayloadAsync(campaign);
        return Ok(executionPayload);
    }

    [HttpGet("channels")]
    public ActionResult<object> GetChannelTypes()
    {
        var types = Enum.GetValues<ChannelType>()
            .Select(type => new { 
                Value = (int)type, 
                Name = type.ToString(),
                DisplayName = GetDisplayName(type)
            })
            .ToList();

        return Ok(types);
    }

    private static string GetDisplayName(ChannelType type) => type switch
    {
        ChannelType.Email => "Email Marketing",
        ChannelType.SMS => "SMS Messages",
        ChannelType.Push => "Push Notifications",
        ChannelType.WhatsApp => "WhatsApp Business",
        ChannelType.Voice => "Voice Calls",
        ChannelType.Messenger => "Facebook Messenger",
        ChannelType.Ads => "Digital Advertising",
        _ => type.ToString()
    };
}

public record GenerateCampaignRequest(string Prompt, List<Guid> DataSourceIds, List<ChannelType> PreferredChannels);