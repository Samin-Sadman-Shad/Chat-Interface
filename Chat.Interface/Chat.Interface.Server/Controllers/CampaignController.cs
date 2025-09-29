using Microsoft.AspNetCore.Mvc;
using MediatR;
using Chat.Interface.Server.Application.Commands;
using Chat.Interface.Server.Domain.Entities;

namespace Chat.Interface.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<Campaign>> GenerateCampaign([FromBody] GenerateCampaignRequest request)
    {
        var command = new GenerateCampaignCommand(request.Prompt, request.DataSourceIds, request.PreferredChannels);
        var campaign = await _mediator.Send(command);
        return Ok(campaign);
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