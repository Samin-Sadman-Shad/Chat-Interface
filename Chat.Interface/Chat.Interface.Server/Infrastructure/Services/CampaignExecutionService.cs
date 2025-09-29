using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Models;

namespace Chat.Interface.Server.Infrastructure.Services;

public interface ICampaignExecutionService
{
    Task<CampaignExecutionPayload> GenerateExecutionPayloadAsync(Campaign campaign);
    Task<string> StreamExecutionPayloadAsync(Campaign campaign);
}

public class CampaignExecutionService : ICampaignExecutionService
{
    public async Task<CampaignExecutionPayload> GenerateExecutionPayloadAsync(Campaign campaign)
    {
        // Simulate processing time
        await Task.Delay(1000);

        var payload = new CampaignExecutionPayload
        {
            CampaignId = campaign.Id,
            CampaignName = campaign.Name,
            Timing = new ExecutionTiming
            {
                ScheduledTime = campaign.Timing.OptimalTime ?? DateTime.UtcNow.AddHours(1),
                TimeZone = campaign.Timing.TimeZone,
                DelayBetweenChannels = 15,
                OptimalDays = campaign.Timing.PreferredDays,
                OptimalTimeOfDay = campaign.Timing.PreferredTimeOfDay ?? TimeSpan.FromHours(10)
            },
            Channels = campaign.Channels.Select((channel, index) => new ChannelExecution
            {
                Type = channel,
                Name = GetChannelDisplayName(channel),
                Configuration = GetChannelConfiguration(channel),
                Message = GenerateMessageVariant(campaign.Message, channel),
                Priority = index + 1,
                Budget = CalculateChannelBudget(channel, campaign.Audience.EstimatedSize)
            }).ToList(),
            Audience = new AudienceExecution
            {
                SegmentName = campaign.Audience.Name,
                TargetingCriteria = ExtractTargetingCriteria(campaign.Audience.Criteria),
                Demographics = ExtractDemographics(campaign.Audience.Criteria),
                Behaviors = ExtractBehaviors(campaign.Audience.Criteria),
                Interests = ExtractInterests(campaign.Audience.Criteria),
                ExclusionCriteria = GenerateExclusionCriteria(),
                EstimatedReach = campaign.Audience.EstimatedSize
            },
            Content = new ContentExecution
            {
                Theme = ExtractTheme(campaign.Message),
                Tone = "Professional and engaging",
                Keywords = ExtractKeywords(campaign.Message),
                Variables = new Dictionary<string, string>
                {
                    ["customerName"] = "{{customer.firstName}}",
                    ["companyName"] = "{{company.name}}",
                    ["productName"] = "{{product.name}}",
                    ["offerExpiry"] = "{{offer.expiryDate}}"
                },
                ABTest = new ABTestConfiguration
                {
                    Enabled = true,
                    Variants = new[] { "Variant A", "Variant B" }.ToList(),
                    TrafficSplit = 0.5,
                    SuccessMetric = "click_through_rate"
                }
            },
            Metadata = new Dictionary<string, object>
            {
                ["generatedAt"] = DateTime.UtcNow,
                ["version"] = "1.0",
                ["executionMode"] = "automated",
                ["approvalRequired"] = false,
                ["estimatedDeliveryTime"] = "2-4 hours",
                ["complianceChecked"] = true,
                ["budgetApproved"] = true
            }
        };

        return payload;
    }

    public async Task<string> StreamExecutionPayloadAsync(Campaign campaign)
    {
        var payload = await GenerateExecutionPayloadAsync(campaign);
        return System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private string GetChannelDisplayName(ChannelType channel) => channel switch
    {
        ChannelType.Email => "Email Marketing",
        ChannelType.SMS => "SMS Campaign",
        ChannelType.Push => "Push Notifications",
        ChannelType.WhatsApp => "WhatsApp Business",
        ChannelType.Voice => "Voice Campaign",
        ChannelType.Messenger => "Facebook Messenger",
        ChannelType.Ads => "Digital Advertising",
        _ => channel.ToString()
    };

    private Dictionary<string, object> GetChannelConfiguration(ChannelType channel) => channel switch
    {
        ChannelType.Email => new Dictionary<string, object>
        {
            ["fromName"] = "Your Company",
            ["fromEmail"] = "campaigns@yourcompany.com",
            ["trackOpens"] = true,
            ["trackClicks"] = true,
            ["suppressionList"] = "global_suppression"
        },
        ChannelType.SMS => new Dictionary<string, object>
        {
            ["shortCode"] = "12345",
            ["messagingService"] = "promotional",
            ["optOutKeywords"] = new[] { "STOP", "UNSUBSCRIBE" }
        },
        ChannelType.Ads => new Dictionary<string, object>
        {
            ["platform"] = "Meta",
            ["objective"] = "conversions",
            ["bidStrategy"] = "lowest_cost",
            ["placement"] = "automatic"
        },
        _ => new Dictionary<string, object>()
    };

    private MessageVariant GenerateMessageVariant(string baseMessage, ChannelType channel)
    {
        var variant = new MessageVariant
        {
            Body = AdaptMessageForChannel(baseMessage, channel),
            CallToAction = GetChannelCTA(channel),
            Personalization = new Dictionary<string, string>
            {
                ["greeting"] = "Hi {{firstName}}",
                ["company"] = "{{companyName}}",
                ["product"] = "{{productName}}"
            }
        };

        if (channel == ChannelType.Email)
        {
            variant.Subject = GenerateEmailSubject(baseMessage);
            variant.Assets = new List<MediaAsset>
            {
                new MediaAsset
                {
                    Type = "image",
                    Url = "https://cdn.example.com/campaign-hero.jpg",
                    Alt = "Campaign Hero Image",
                    Properties = new Dictionary<string, object> { ["width"] = 600, ["height"] = 300 }
                }
            };
        }

        return variant;
    }

    private string AdaptMessageForChannel(string message, ChannelType channel) => channel switch
    {
        ChannelType.SMS => TruncateForSMS(message),
        ChannelType.Push => TruncateForPush(message),
        ChannelType.Email => AddEmailFormatting(message),
        _ => message
    };

    private string TruncateForSMS(string message) => 
        message.Length > 160 ? message.Substring(0, 157) + "..." : message;

    private string TruncateForPush(string message) => 
        message.Length > 100 ? message.Substring(0, 97) + "..." : message;

    private string AddEmailFormatting(string message) => 
        $"<html><body><div style='font-family: Arial, sans-serif;'>{message}</div></body></html>";

    private string GenerateEmailSubject(string message)
    {
        if (message.Contains("sale", StringComparison.OrdinalIgnoreCase))
            return "🎉 Exclusive Sale - Limited Time Only!";
        if (message.Contains("welcome", StringComparison.OrdinalIgnoreCase))
            return "Welcome to the family! 👋";
        return "Personalized offer just for you";
    }

    private string GetChannelCTA(ChannelType channel) => channel switch
    {
        ChannelType.Email => "Shop Now",
        ChannelType.SMS => "Reply YES",
        ChannelType.Push => "Open App",
        ChannelType.Ads => "Learn More",
        _ => "Get Started"
    };

    private decimal CalculateChannelBudget(ChannelType channel, int audienceSize) => channel switch
    {
        ChannelType.Ads => audienceSize * 0.05m, // $0.05 per person for ads
        ChannelType.Email => audienceSize * 0.001m, // $0.001 per email
        ChannelType.SMS => audienceSize * 0.02m, // $0.02 per SMS
        _ => 0m
    };

    private List<string> ExtractTargetingCriteria(Dictionary<string, object> criteria)
    {
        var targeting = new List<string>();
        
        if (criteria.ContainsKey("ageRange"))
            targeting.Add($"Age: {criteria["ageRange"]}");
        if (criteria.ContainsKey("location"))
            targeting.Add($"Location: {criteria["location"]}");
        if (criteria.ContainsKey("deviceType"))
            targeting.Add($"Device: {criteria["deviceType"]}");
        
        return targeting;
    }

    private Dictionary<string, object> ExtractDemographics(Dictionary<string, object> criteria)
    {
        return new Dictionary<string, object>
        {
            ["ageRange"] = criteria.GetValueOrDefault("ageRange", "25-45"),
            ["gender"] = "All",
            ["income"] = "Middle to High",
            ["education"] = "College+"
        };
    }

    private Dictionary<string, object> ExtractBehaviors(Dictionary<string, object> criteria)
    {
        return new Dictionary<string, object>
        {
            ["purchaseHistory"] = criteria.GetValueOrDefault("behavior", "engaged_last_30_days"),
            ["websiteActivity"] = "High engagement",
            ["emailEngagement"] = "Active subscriber",
            ["socialMediaActivity"] = "Regular user"
        };
    }

    private Dictionary<string, object> ExtractInterests(Dictionary<string, object> criteria)
    {
        var interests = criteria.GetValueOrDefault("interests", new[] { "technology", "shopping" });
        return new Dictionary<string, object>
        {
            ["primary"] = interests,
            ["secondary"] = new[] { "lifestyle", "entertainment", "travel" },
            ["affinity"] = new[] { "early_adopters", "brand_loyal", "price_conscious" }
        };
    }

    private List<string> GenerateExclusionCriteria()
    {
        return new List<string>
        {
            "Recently purchased",
            "Unsubscribed users",
            "Competitors",
            "Internal employees"
        };
    }

    private string ExtractTheme(string message)
    {
        if (message.Contains("sale", StringComparison.OrdinalIgnoreCase) || 
            message.Contains("discount", StringComparison.OrdinalIgnoreCase))
            return "promotional";
        if (message.Contains("welcome", StringComparison.OrdinalIgnoreCase))
            return "onboarding";
        return "engagement";
    }

    private List<string> ExtractKeywords(string message)
    {
        var keywords = new List<string>();
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var word in words)
        {
            if (word.Length > 4 && !CommonWords.Contains(word.ToLower()))
                keywords.Add(word.ToLower());
        }
        
        return keywords.Take(10).ToList();
    }

    private static readonly HashSet<string> CommonWords = new()
    {
        "the", "and", "for", "are", "but", "not", "you", "all", "can", "her", "was", "one", "our", "had", "but", "has"
    };
}