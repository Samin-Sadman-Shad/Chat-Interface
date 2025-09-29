using Chat.Interface.Server.Domain.Entities;

namespace Chat.Interface.Server.Models;

public class CampaignExecutionPayload
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public ExecutionTiming Timing { get; set; } = new();
    public List<ChannelExecution> Channels { get; set; } = new();
    public AudienceExecution Audience { get; set; } = new();
    public ContentExecution Content { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ExecutionTiming
{
    public DateTime ScheduledTime { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public int DelayBetweenChannels { get; set; } // in minutes
    public List<string> OptimalDays { get; set; } = new();
    public TimeSpan OptimalTimeOfDay { get; set; }
}

public class ChannelExecution
{
    public ChannelType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
    public MessageVariant Message { get; set; } = new();
    public int Priority { get; set; }
    public decimal Budget { get; set; }
}

public class MessageVariant
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string CallToAction { get; set; } = string.Empty;
    public List<MediaAsset> Assets { get; set; } = new();
    public Dictionary<string, string> Personalization { get; set; } = new();
}

public class MediaAsset
{
    public string Type { get; set; } = string.Empty; // image, video, document
    public string Url { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class AudienceExecution
{
    public string SegmentName { get; set; } = string.Empty;
    public List<string> TargetingCriteria { get; set; } = new();
    public Dictionary<string, object> Demographics { get; set; } = new();
    public Dictionary<string, object> Behaviors { get; set; } = new();
    public Dictionary<string, object> Interests { get; set; } = new();
    public List<string> ExclusionCriteria { get; set; } = new();
    public int EstimatedReach { get; set; }
}

public class ContentExecution
{
    public string Theme { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public Dictionary<string, string> Variables { get; set; } = new();
    public ABTestConfiguration ABTest { get; set; } = new();
}

public class ABTestConfiguration
{
    public bool Enabled { get; set; }
    public List<string> Variants { get; set; } = new();
    public double TrafficSplit { get; set; }
    public string SuccessMetric { get; set; } = string.Empty;
}