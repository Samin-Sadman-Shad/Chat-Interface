using Microsoft.AspNetCore.Mvc;
using MediatR;
using Chat.Interface.Server.Application.Commands;
using Chat.Interface.Server.Application.Queries;
using Chat.Interface.Server.Domain.Entities;

namespace Chat.Interface.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataSourcesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DataSourcesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<DataSource>>> GetDataSources()
    {
        var dataSources = await _mediator.Send(new GetDataSourcesQuery());
        return Ok(dataSources);
    }

    [HttpPost("connect")]
    public async Task<ActionResult<Guid>> ConnectDataSource([FromBody] ConnectDataSourceRequest request)
    {
        var command = new ConnectDataSourceCommand(request.Type, request.Name, request.Configuration);
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpGet("types")]
    public ActionResult<object> GetDataSourceTypes()
    {
        var types = Enum.GetValues<DataSourceType>()
            .Select(type => new { 
                Value = (int)type, 
                Name = type.ToString(),
                DisplayName = GetDisplayName(type)
            })
            .ToList();

        return Ok(types);
    }

    private static string GetDisplayName(DataSourceType type) => type switch
    {
        DataSourceType.GTM => "Google Tag Manager",
        DataSourceType.FacebookPixel => "Facebook Pixel",
        DataSourceType.GoogleAds => "Google Ads",
        DataSourceType.FacebookPage => "Facebook Page",
        DataSourceType.Website => "Website Analytics",
        DataSourceType.Shopify => "Shopify Store",
        DataSourceType.CRM => "Customer CRM",
        DataSourceType.TwitterPage => "Twitter Page",
        DataSourceType.ReviewSites => "Review Sites",
        DataSourceType.AdManager => "Ad Manager",
        _ => type.ToString()
    };
}

public record ConnectDataSourceRequest(DataSourceType Type, string Name, Dictionary<string, object> Configuration);