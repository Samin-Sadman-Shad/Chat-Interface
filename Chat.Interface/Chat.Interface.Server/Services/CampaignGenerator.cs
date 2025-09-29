using Chat.Interface.Server.Domain.Entities;

namespace Chat.Interface.Server.Infrastructure.Services;

public interface ICampaignGenerator
{
    Task<Campaign> GenerateAsync(string prompt, List<DataSource> dataSources, List<ChannelType> preferredChannels);
}

public class CampaignGenerator : ICampaignGenerator
{
    public async Task<Campaign> GenerateAsync(string prompt, List<DataSource> dataSources, List<ChannelType> preferredChannels)
    {
        // Simulate AI-powered campaign generation
        await Task.Delay(2000);

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = $"AI Generated Campaign - {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Message = GenerateMessageFromPrompt(prompt),
            Channels = DetermineOptimalChannels(preferredChannels, dataSources),
            Audience = GenerateAudienceSegment(dataSources),
            Timing = GenerateOptimalTiming(),
            Status = CampaignStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                ["originalPrompt"] = prompt,
                ["dataSourceIds"] = dataSources.Select(ds => ds.Id).ToList(),
                ["generatedAt"] = DateTime.UtcNow,
                ["confidence"] = 0.85
            }
        };

        return campaign;
    }

    private string GenerateMessageFromPrompt(string prompt)
    {
        // Simulate AI message generation based on prompt
        return prompt.Contains("sale", StringComparison.OrdinalIgnoreCase)
            ? "🎉 Limited Time Sale! Don't miss out on exclusive deals tailored just for you!"
            : prompt.Contains("welcome", StringComparison.OrdinalIgnoreCase)
            ? "Welcome to our community! We're excited to have you on board."
            : $"Personalized message based on your interests and behavior patterns.";
    }

    private List<ChannelType> DetermineOptimalChannels(List<ChannelType> preferred, List<DataSource> dataSources)
    {
        var optimal = new List<ChannelType>(preferred);
        
        // Add email if we have website or CRM data
        if (dataSources.Any(ds => ds.Type is DataSourceType.Website or DataSourceType.CRM) && 
            !optimal.Contains(ChannelType.Email))
        {
            optimal.Add(ChannelType.Email);
        }

        // Add ads if we have pixel data
        if (dataSources.Any(ds => ds.Type is DataSourceType.FacebookPixel or DataSourceType.GoogleAds) && 
            !optimal.Contains(ChannelType.Ads))
        {
            optimal.Add(ChannelType.Ads);
        }

        return optimal.Take(3).ToList(); // Limit to 3 channels for better focus
    }

    private AudienceSegment GenerateAudienceSegment(List<DataSource> dataSources)
    {
        return new AudienceSegment
        {
            Name = "High-Intent Customers",
            Criteria = new Dictionary<string, object>
            {
                ["ageRange"] = "25-45",
                ["interests"] = new[] { "technology", "shopping", "lifestyle" },
                ["behavior"] = "engaged_last_30_days",
                ["location"] = "US",
                ["deviceType"] = "mobile_preferred"
            },
            EstimatedSize = Random.Shared.Next(1000, 50000)
        };
    }

    private CampaignTiming GenerateOptimalTiming()
    {
        return new CampaignTiming
        {
            OptimalTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10), // Tomorrow at 10 AM
            TimeZone = "UTC",
            PreferredDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday" }.ToList(),
            PreferredTimeOfDay = TimeSpan.FromHours(10) // 10 AM
        };
    }
}